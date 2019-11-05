#-*-coding:utf-8-*-
'''
Author tony_dong@zhbojay.com
V1.0   2019/9/9
for zebra device

V1.1	2019/10/21
compatible with python2 and python3 interpreter
'''
import clr
import re
import time
import binascii
import platform

#**********Interop.CoreScanner.dll****************#
# Zebradll = ctypes.cdll.LoadLibrary
# Zebradll("CoreScanner.dll")
# from CoreScanner import *
# instance = CCoreScannerClass()
# print dir(instance)
#**********Interop.CoreScanner.dll****************#
clr.FindAssembly('BojayScanner.dll')
from BojayScanner import *
instance = CoreScannerClass()

# auto get python interpreter
Interpreter = platform.python_version()[0]
print(Interpreter)

class BojayScannerClass:

    # open scanner device
    def OpenDevice(self):
        instance.OpenScanner()

    # close scaner device
    def CloseDevice(self):
        instance.CloseScanner()

    # pull trigger and get data
    def PullTriggerAndGetData(self,timeout = 10):
        content = instance.PullTrigger(timeout)
        if content == 'Register event for barcode fail' or content == 'timeout':
            return -1
        print(content)
        pattern = re.compile(r'<datalabel>(.*?)</datalabel>', re.S)
        results = re.findall(pattern, content)
        if Interpreter == '3':
            results = re.sub('\'|\[|]', '', str(results))
        else:
            results = re.sub('\'|\[u|]' , '',str(results))
        results = results.split(' ')
        data = b''
        for index in range(len(results)):
            strTemp = (results[index].strip())[2:]
            if Interpreter == '3':
                data = data + binascii.unhexlify(strTemp)
            else:
                data = data + binascii.unhexlify(strTemp)
        if Interpreter == '3':
            data = data.decode()
        return data

    # release trigger
    def ReleaseTrigger(self):
        ret = instance.ReleaseTrigger()
        if ret != 0:
            return -1
        return 0

if __name__ == '__main__':
    api = BojayScannerClass()
    api.OpenDevice()
    print(api.PullTriggerAndGetData(10))
    print(api.ReleaseTrigger())
    api.CloseDevice()