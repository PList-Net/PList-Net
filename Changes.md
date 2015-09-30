# Change log

## 2.0.1
 - corrected bug in XML writer that resulted in written file missing the root "plist" element

## 1.x - 2.0
 - root namespace changed from `CE.iPhone.x` to `PListNet`
 - moved all nodes to the `PListNet.Nodes` namespace
 - renamed nodes from PListXXX to XXXNode (e.g. `PListArray` => `ArrayNode`)
 - dramatically reduced public API surface -- things meant to be internal now are
