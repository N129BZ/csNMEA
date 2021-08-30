# csNMEA
This is a very simplistic implementation of b3nn0's EU Stratux gps.go ublox NMEA sentence configuration, ultimately processed through 101100's nmea-simple codecs, with the addition of json output. Both the go and js codes have been ported to C#, using the linux versions of .NET Core and vscode running on Ubuntu MATE 20.04. 

##
Hardware requirement: 
    UBLOX usb gps dongle or equivalent, see https://www.ebay.com/p/21037983465 also available on Amazon.

Software requirements, 3 nuget packages:
    Newtonsoft.Json --version 13.0.1
    System.IO.Ports --version 5.0.1
    System.Management --version 5.0.0

###

Please see: 

https://github.com/b3nn0/stratux/blob/master/main/gps.go

https://github.com/101100/nmea-simple/tree/master/dist/codecs

