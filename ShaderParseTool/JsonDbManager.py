try:
    import json  # python >= 2.6
except ImportError:
    import simplejson as json
import codecs

__author__ = 'wangguojin'


class ShaderStructModel(object):
    name = ''
    file = ''
    variables = None

    def __init__(self):
        self.name = ''
        self.file = ''
        self.variables = {}


class ShaderFunctionModel(object):
    name = ''
    file = ''
    variables = None
    def __init__(self):
        self.name = ''
        self.file = ''
        self.variables = {}


class ShaderMacrosModel(object):
    name    =   ''
    file = ''
    # {'keyword':[float/desc,selfdesc]}
    variables = None

    def __init__(self):
        self.name = ''
        self.file = ''
        self.variables = {}

class ShaderInerVarModel(object):
    name    =   ''
    # {'keyword':[float,selfdesc]}
    variables = None

    def __init__(self):
        self.name = ''
        self.variables = {}

class ShaderModelInfo(object):
    head_sets   = []
    struct_sets = []
    func_sets   = []
    macro_sets  = []
    variner_sets = []

    def __init__(self):
        self.head_sets      = []
        self.struct_sets    = []
        self.func_sets      = []
        self.macro_sets     = []
        self.variner_sets   = []

    def validate(self):
        if len(self.head_sets) < 1 or len(self.struct_sets) < 1:
            return False
        return True

    def is_struct(self,target):
        for st in self.struct_sets:
            if st.name == target:
                return True
        return False

    def is_macro(self,target):
        for mc in self.macro_sets:
            if mc.name == target:
                return  True
        return False


class JsonDb(object):
    def __init__(self):
        pass

    def object2dict(self, obj):
        d = {}
        d.update(obj.__dict__)
        return d

    def savemodel2file(self,obj,name):
        if isinstance(obj,ShaderModelInfo):
            if obj.validate() == False:
                return
            with codecs.open(name + '.json','w','utf-8') as file:
                dump = json.dumps(obj,default=self.object2dict,indent=2)
                file.write(dump)
                print 'final save model tags :', dump
        else:
            pass

if __name__ == '__main__':
    model = ShaderModelInfo()
    st1 = ShaderStructModel()
    st1.name = 'app_base'
    st1.variables['light'] = ['float','test']
    st1.variables['tex'] = ['sampler2D','texture']
    model.struct_sets.append(st1)
    st2 = ShaderStructModel()
    st2.name = 'app_basefull'
    st2.variables['light2'] = ['float','test']
    st2.variables['tex2'] = ['sampler2D','texture']
    model.struct_sets.append(st2)
    model.head_sets.append('light.cginc')
    model.head_sets.append('hlsl.cginc')
    jdb = JsonDb()
    jdb.savemodel2file(model,'test')
