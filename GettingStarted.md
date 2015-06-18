# Important Types #

| **Type** | **PList XML Representation** | **PList binary Typecode** | **Description** |
|:---------|:-----------------------------|:--------------------------|:----------------|
| IPListElement | ---                          | ---                       | Common Interface for PList elements |
| PListRoot | `<plist Version="1.0">`      | ---                       | Root element of a plist |
| PListArray | `<array>`                    | 0x0A                      | An array container |
| PListDict | `<dict>`                     | 0x0D                      | A dictionary container (Key - Value Collection)|
| PListBool | `<true/>` / `<false/>`       | 0x00 (Length 0x08 / 0x09) | A boolean type  |
| PListData | `<data>`                     | 0x04                      | A binary data element (byte`[``]`) |
| PListDate | `<date>`                     | 0x03                      | A Date/Time element |
| PListFill | `<fill/>`                    | 0x00 (Length 0x0F)        | A fill element (not used in Xml) |
| PListInteger | `<integer>`                  | 0x01                      | An integer number element (8, 16, 32 or 64 Bit) |
| PListNull | `<null/>`                    | 0x00 (Lenght 0x00)        | A null element  |
| PListReal | `<real>`                     | 0x02                      | A floating point number (32 or 64 Bit) |
| PListString | `<string>` / `<ustring>`     | 0x05 / 0x06               | A string, encoded as UTF-8 or UTF-16BE |



# Usage #

```
// Autodetects Binary / Xml
PListRoot myPlist = PListRoot.Load("foo.plist");

// ...
// do anything with the plist
// ...

// Saves the plist in the Format it was read 
// (stored in myPlist.Format
myPlist.Save("bar.plist"); 

myPlist.Save("bar.binaty.plist", PListFormat.Binary);
myPlist.Save("bar.xml.plist", PListFormat.Xml);
```