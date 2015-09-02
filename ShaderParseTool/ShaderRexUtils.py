import re
import os
from ShaderConsts import ShaderConst

__author__ = 'wangguojin'

class HeaderHandleInfo(object):
    headers = []
    handled = False

    def __init__(self):
        pass
    def __init__(self,headers,ok):
        self.headers = headers
        self.handled = ok

class ShaderRex(object):
    __incRexStr         = '^#\s*include\s+"\w+.cginc"'
    __structRexStr      = '\s*struct\s+\w+\s*{?'
    __funcRexStr        = '(\w)+\s+(\w)+\s*\(((\s*\w)*,?)*'
    __macroRexStr       = '#\s*define'
    __varRexStr         = '(\w+\s+){1,2}_\w+(\[\w+\])*;'

    def __init__(self):
        self.__incRex    = re.compile(self.__incRexStr)
        self.__structRex = re.compile(self.__structRexStr)
        self.__funcRex   = re.compile(self.__funcRexStr)
        self.__macroRex  = re.compile(self.__macroRexStr)
        self.__varRex    = re.compile(self.__varRexStr)

    def getincinfile(self, filepath):
        fileHandle = None
        incLists = []
        try:
            fileHandle = open(filepath, 'r')
            vallist = fileHandle.readlines()
            for val in vallist:
                ctx = self.__incRex.findall(val.strip('\n'))
                if ctx:
                    L = []
                    for inc in ctx:
                        tmpStr = re.split('[\"\s]+',inc)
                        L = [x for x in tmpStr if x.endswith('.cginc')]
                    if len(L) > 0:
                        incLists += L
        finally:
            if fileHandle:
                fileHandle.close()
        return incLists

    def getstructinfile(self,ctxlines):
        rtctx = []
        startidx = -1
        idxcount = 0
        for ctx in ctxlines:
            notrip = str(ctx).strip('\n')
            if startidx == -1:
                mch = self.__structRex.match(notrip)
                if mch:
                    startidx = idxcount
            mch = re.match('\s*};',notrip)
            if mch and startidx != -1:
                rtctx.append([startidx,idxcount])
                startidx = -1
            idxcount = idxcount + 1
        return rtctx


    def getfuncinfile(self,ctxlines,model):
        sdconst = ShaderConst()
        rtctx = []
        idx = 0
        maxidx = len(ctxlines)
        while idx < maxidx:
            ctx = ctxlines[idx]
            notrip = str(ctx).strip(' \t\n')
            notrip = notrip.lstrip('inline ')
            mh = self.__funcRex.match(notrip)
            if mh:
                prefixvar = notrip.split(' ',3)
                if sdconst.isbasetype(prefixvar[0]) or model.is_struct(prefixvar[0]):
                    left = re.findall('\(',notrip)
                    right = re.findall('\)',notrip)
                    if left and right and len(left) == len(right):
                        srightidx = prefixvar[1].find('(')
                        if srightidx > 0:
                            func = [prefixvar[0],prefixvar[1][:srightidx].strip(' ')]
                        else:
                            func = [prefixvar[0],prefixvar[1].strip(' ')]
                        pm = []
                        pmrex = notrip[notrip.index('('):notrip.index(')')]
                        pmsplit = re.split('[\(\)\,\s]+',pmrex)
                        tidx = 0
                        while tidx < len(pmsplit):
                           if sdconst.isbasetype(pmsplit[tidx]) or model.is_struct(pmsplit[tidx]):
                                pm.append([pmsplit[tidx],pmsplit[tidx + 1]])
                                tidx += 2
                           elif sdconst.isparamprefix(pmsplit[tidx]):
                                pm.append([pmsplit[tidx] + ' ' + pmsplit[tidx + 1],  pmsplit[tidx + 2]])
                                tidx += 3
                           else:
                               tidx = tidx + 1
                        func.append(pm)
                        rtctx.append(func)
                    else:
                        #not in one line max find 10 times
                        func = [prefixvar[0],prefixvar[1]]
                        pm = []
                        tidx = 0
                        while tidx < 10:
                            tpidx = idx + tidx
                            if prefixvar:
                                ctx = str(prefixvar[2]).strip(' \t\n')
                                prefixvar = None
                            else:
                                ctx = str(ctxlines[tpidx]).strip(' \t\n')
                            pmsplit = re.split('[\(\)\,\s]+',ctx)
                            vidx = 0
                            while vidx < len(pmsplit):
                                if sdconst.isbasetype(pmsplit[vidx]):
                                    pm.append([pmsplit[vidx],pmsplit[vidx + 1]])
                                    vidx += 2
                                elif sdconst.isparamprefix(pmsplit[vidx]):
                                    pm.append([pmsplit[vidx] + ' ' + pmsplit[vidx + 1],  pmsplit[vidx + 2]])
                                    vidx += 3
                                else:
                                   vidx = vidx + 1
                            right = re.findall('\)',ctx)
                            if right and len(right) == 1:
                                break
                            tidx += 1
                        func.append(pm)
                        rtctx.append(func)
                        idx = tpidx + 1
                        continue
                else:
                    pass
            idx += 1

        return rtctx


    def getmacroinfile(self,ctxlines,model):
        sdconst = ShaderConst()
        rtctx = []
        idx = 0
        maxidx = len(ctxlines)
        while idx < maxidx:
            ctx1 = str(ctxlines[idx]).strip(' \t\n')
            mh = self.__macroRex.match(ctx1)
            if mh:
                #     get name
                macs = []
                subidx = ctx1.index('define')
                subdef = ctx1[subidx+6:].strip(' ')
                subidx = subdef.find(')')
                if subidx > 0:
                    name = subdef[:subidx+1]
                    desc = subdef[subidx+1:].strip(' \\')
                else:
                    vals = re.split('[\s\t]+',subdef)
                    if len(vals) > 1:
                        name = vals[0]
                        desc = subdef[subdef.index(name)+len(name):].strip(' \\')
                    else:
                        name = subdef.strip(' \\')
                        desc = ''
                if sdconst.isbasetype(name) or model.is_struct(name):
                    idx += 1
                    continue
                macs.append(name)
                macs.append(desc)
                rtctx.append(macs)
            idx += 1
        return rtctx


    def getinervarinfile(self,ctxlines,model):
        sdconst = ShaderConst()
        rtctx = []
        idx = 0
        maxidx = len(ctxlines)
        while idx < maxidx:
            ctx1 = str(ctxlines[idx]).strip('\n')
            mh = self.__varRex.match(ctx1)
            if mh:
                #     get name
                vars = []
                vals = re.split('\s+',ctx1)
                if sdconst.isvariableprefix(vals[0]):
                    name = vals[2].strip(' \t;')
                    desc = vals[1].strip(' \t')
                elif sdconst.isbasetype(vals[0]) or model.is_struct(vals[0]) or model.is_macro(vals[0]):
                    name = vals[1].strip(' \t;')
                    desc = vals[0].strip(' \t')
                else:
                    idx += 1
                    continue
                vars.append(name)
                vars.append(desc)
                rtctx.append(vars)
            idx += 1
        return rtctx