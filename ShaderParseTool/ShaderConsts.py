__author__ = 'wangguojin'


class ShaderConst(object):
    #key for callback
    keysharp    = "#"
    keystruct   = "struct"
    keyfunction = "function"
    keymacro    = "macros"
    keyvariable = "inervar"

    #tag for variable name
    tagvariables = ['uniform','void','int','bool','float','float2','float3','float4',
                    'float2x2','float3x3','float4x4',
                    'fixed','fixed2','fixed3','fixed4','fixed2x2','fixed3x3','fixed4x4'
                    'half','half2','half3','half4','half2x2','half3x3','half4x4']
    tagparamprefix = ['in','out','inout']

    def __init__(self):
        pass

    def isbasetype(self,tag):
        if tag in self.tagvariables:
            return True
        return False

    def isparamprefix(self,tag):
        if tag in self.tagparamprefix:
            return True
        return False

    def isvariableprefix(self,tag):
        if tag == 'uniform':
            return True
        return  False

    @staticmethod
    def taghandle(param):
        pass


    tagmap = {keysharp:     taghandle,
              keystruct:    taghandle,
              keyfunction:  taghandle,
              keyvariable:  taghandle,
              keymacro:     taghandle}
