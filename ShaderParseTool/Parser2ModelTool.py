import ShaderConsts
from JsonDbManager import ShaderStructModel
from JsonDbManager import ShaderFunctionModel
from JsonDbManager import ShaderMacrosModel
from JsonDbManager import ShaderInerVarModel

import re

__author__ = 'wangguojin'

class Parser2ModelManager(object):
    def __init__(self):
        pass

    @staticmethod
    def parse_sharp(param):
        print 'test #',param

    @staticmethod
    def parse_struct(ctxidx,filectx):
        if len(ctxidx) < 1:
            return None
        st = ctxidx[0]
        ed = ctxidx[1]
        model = ShaderStructModel()
        valall =''
        while st <= ed:
            valall += str(filectx[st]).strip('\t\n')
            st += 1
        # get struct name
        head = re.match(r'^struct\s+\w+\s*\{',valall)
        if head:
            hds = re.split(r'\s+',valall[head.start():head.end()])
            if len(hds) == 3:
                model.name = hds[1]
            else:
                return None
        else:
            return None
        # get iner variables
        iners = re.findall("\s*\w+\s+\w+\s*(?:\:\s*\w+;|;)",valall)
        for val in iners:
            hds1 = val.strip(' ;')
            hds = re.split(r'[\s\:\;]*',hds1)
            if len(hds) == 2:
                model.variables[hds[1]] = [hds[0],'null']
            elif len(hds) == 3:
                model.variables[hds[1]] = [hds[0],hds[2]]
        return model

    @staticmethod
    def parse_function(fndata):
        # data is [float4,fun_name,[[float,pm1],[float,pm2]]
        if len(fndata) < 1:
            return None
        model = ShaderFunctionModel()
        model.name = fndata[1]
        model.variables['ret']  = fndata[0]
        model.variables['par']  = fndata[2]
        return model

    @staticmethod
    def parse_macro(mcdata):
        if len(mcdata) < 1:
            return  None
        model = ShaderMacrosModel()
        model.name = mcdata[0]
        model.variables['desc'] = mcdata[1]
        return  model


    @staticmethod
    def parse_inervar(ivdata):
        if len(ivdata) < 1:
            return  None
        model = ShaderInerVarModel()
        model.name = ivdata[0]
        model.variables['desc'] = ivdata[1]
        return model

    def registerfunc(self,constobj):
        assert(isinstance(constobj,ShaderConsts.ShaderConst))
        constobj.tagmap[constobj.keysharp]      = self.parse_sharp
        constobj.tagmap[constobj.keystruct]     = self.parse_struct
        constobj.tagmap[constobj.keyfunction]   = self.parse_function
        constobj.tagmap[constobj.keymacro]      = self.parse_macro
        constobj.tagmap[constobj.keyvariable]   = self.parse_inervar