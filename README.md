# csNMEA
This is a very simplistic implementation of b3nn0's EU version of the Stratux file gps.go, which sets the configurations of a UBLOX usb serial device to receive NMEA sentences as configured. The received sentences are then processed through 101100's nmea-simple codecs, which subsequently output (my addition) formatted json. 

Both the go and js codes have been ported to C#, using the linux versions of .NET Core and vscode running on Ubuntu MATE 20.04. 

###
Hardware requirement:
    UBLOX usb gps dongle or equivalent, see https://www.ebay.com/p/21037983465 also available on Amazon. (I have no affiliation with either.)  Search "Stratux GPS" 



###
Software requirements, add 3 nuget packages:

    dotnet add package Newtonsoft.Json --version 13.0.1
    dotnet add package System.IO.Ports --version 5.0.0
    dotnet add package System.Management --version 5.0.0

###

Please see:

https://github.com/b3nn0/stratux/blob/master/main/gps.go

https://github.com/101100/nmea-simple/tree/master/dist/codecs

