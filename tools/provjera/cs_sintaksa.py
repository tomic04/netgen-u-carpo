import sys, re, collections
import tree_sitter_c_sharp as tscs
from tree_sitter import Language, Parser
L = Language(tscs.language())
P = Parser(L)
def walk(n):
    yield n
    for c in n.children:
        yield from walk(c)
bad=0
for f in sys.argv[1:]:
    src=open(f,'rb').read()
    t=P.parse(src)
    errs=[n for n in walk(t.root_node) if n.type=='ERROR' or n.is_missing]
    for e in errs[:5]:
        print(f, 'SYNTAX', e.type, e.start_point, src[e.start_byte:e.start_byte+60])
    bad+=len(errs)
    # duplicate members per class (method name + param count)
    for cls in [n for n in walk(t.root_node) if n.type=='class_declaration']:
        body=cls.child_by_field_name('body')
        seen=collections.Counter()
        for m in body.children:
            if m.type in ('method_declaration','field_declaration','property_declaration'):
                if m.type=='method_declaration':
                    name=m.child_by_field_name('name').text.decode()
                    params=m.child_by_field_name('parameters')
                    sig=name+'/'+str(len([p for p in params.children if p.type=='parameter']))
                    # include param types
                    sig=name+'('+','.join(p.child_by_field_name('type').text.decode() for p in params.children if p.type=='parameter')+')'
                elif m.type=='field_declaration':
                    vd=[n for n in walk(m) if n.type=='variable_declarator']
                    for v in vd:
                        seen['F:'+v.child_by_field_name('name').text.decode() if v.child_by_field_name('name') else v.children[0].text.decode()]+=1
                    continue
                else:
                    sig='P:'+m.child_by_field_name('name').text.decode()
                seen[sig]+=1
        for k,v in seen.items():
            if v>1: print(f,'DUP',cls.child_by_field_name('name').text.decode(),k); bad+=1
print('problems',bad)
