# coding=utf-8
import os
from ShaderRexUtils import ShaderRex
from ShaderRexUtils import HeaderHandleInfo
from ShaderConsts import ShaderConst
from Parser2ModelTool import Parser2ModelManager
import JsonDbManager

__author__ = 'wangguojin'

FilePathDir = os.path.curdir + os.sep + 'builtin_shaders-4.6.1' + os.sep + 'CGIncludes'
rexglobal   = ShaderRex()
sdConst     = ShaderConst()
psTool       = Parser2ModelManager()

# build header order
def buildorder(files):
    infoDict = {}
    for file in files:
        L = rexglobal.getincinfile(FilePathDir + os.sep + file)
        info = HeaderHandleInfo(L, False)
        infoDict[file] = info
    return infoDict


def loadcgfiledata(order):
    assert (isinstance(order, dict))
    handleOk = False
    L = []
    while handleOk == False:
        isend = True
        for k in order.keys():
            if order[k].handled == False:
                if len(order[k].headers) > 0:
                    passed = True
                    for x in order[k].headers:
                        if order[x].handled == False:
                            passed = False
                            break
                    if passed:
                        L.append(k)
                        order[k].handled = True
                        isend = False
                else:
                    L.append(k)
                    order[k].handled = True
                    isend = False
        handleOk = isend
    return L


def parsestruct(ctxlines):
    return rexglobal.getstructinfile(ctxlines)


def injectstructdata(data, filectx, model,filehandle):
    # data is [[1,2],[]]
    for st in data:
        stinfo = sdConst.tagmap[sdConst.keystruct](st,filectx)
        if stinfo:
            stinfo.file = filehandle
            model.struct_sets.append(stinfo)
    return


def parsesfunction(ctxlines,model):
    return rexglobal.getfuncinfile(ctxlines,model)


def injectfunctiondata(data,model,filehandle):
    # data is [[1,2,3]]
    for fn in data:
        fninfo = sdConst.tagmap[sdConst.keyfunction](fn)
        if fninfo:
            fninfo.file = filehandle
            model.func_sets.append(fninfo)
    return

def parsesmacros(ctxlines,model):
    return rexglobal.getmacroinfile(ctxlines,model)

def injectmacrosdata(data,model,filehandle):
    # data is [[vname,float/desc]]
    for mc in data:
        mcinfo = sdConst.tagmap[sdConst.keymacro](mc)
        if mcinfo:
            mcinfo.file = filehandle
            model.macro_sets.append(mcinfo)
    return

def parsesinervar(ctxlines,model):
    return rexglobal.getinervarinfile(ctxlines,model)

def injectinervardata(data,model,filehandle):
    # data is [[vname,float/desc]]
    if data == None:
        return
    for mc in data:
        mcinfo = sdConst.tagmap[sdConst.keyvariable](mc)
        if mcinfo:
            mcinfo.file = filehandle
            model.variner_sets.append(mcinfo)
    return

def loadfile2parse(model, files):
    assert (isinstance(model, JsonDbManager.ShaderModelInfo))
    # 1 parser struct info
    for filehandle in files:
        realpath = FilePathDir + os.sep + filehandle
        filehd = open(realpath, 'r')
        ctxlines = filehd.readlines()
        stdata = parsestruct(ctxlines)
        injectstructdata(stdata,ctxlines,model,filehandle)
        filehd.close()
    # 2 parser function info
    for filehandle in files:
        realpath = FilePathDir + os.sep + filehandle
        filehd = open(realpath, 'r')
        ctxlines = filehd.readlines()
        fndata = parsesfunction(ctxlines,model,)
        injectfunctiondata(fndata,model,filehandle)
        filehd.close()
    # 3 parser macros and inter variables info
    for filehandle in files:
        realpath = FilePathDir + os.sep + filehandle
        filehd = open(realpath, 'r')
        ctxlines = filehd.readlines()
        fndata = parsesmacros(ctxlines,model)
        injectmacrosdata(fndata,model,filehandle)
        filehd.close()
    # 4 parser inervariables info
    for filehandle in files:
        realpath = FilePathDir + os.sep + filehandle
        filehd = open(realpath, 'r')
        ctxlines = filehd.readlines()
        fndata = parsesinervar(ctxlines,model)
        injectinervardata(fndata,model,filehandle)
        filehd.close()
    return


mShaderFiles = [file for file in os.listdir(FilePathDir) if os.path.splitext(file)[1] == '.cginc']
print '1、cginc files:', mShaderFiles

headerOrder = buildorder(mShaderFiles)
print '2、cginc file relation :'
for k in headerOrder.keys():
    v = headerOrder[k]
    print k, 'value ', v.headers, ' ', v.handled
incOrder = loadcgfiledata(headerOrder)
print incOrder

print '3、register the parser functions'

psTool.registerfunc(sdConst)

print '4、parser ht cginc file\'s tag to json file'
modelInfo = JsonDbManager.ShaderModelInfo()
# header
for hd in incOrder:
    modelInfo.head_sets.append(hd)
# all tags
loadfile2parse(modelInfo,incOrder)

print '5、save default cginc file tag'
jdb = JsonDbManager.JsonDb()
jdb.savemodel2file(modelInfo, 'unity_shader_tag')
