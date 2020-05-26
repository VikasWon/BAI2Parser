# BAI2Parser
Parse BAI2 format files to C# classes, which can be used to process the data. The parser combines the continuation records(88) to the previous records and warps the data. In case of incorrect file format, an expection is returned with the incorrect warped record.
