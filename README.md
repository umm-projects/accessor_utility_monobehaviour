# What?

* MonoBehaviour に対して型ベースのアクセサ (もどき) を提供します

# Why?

* 個別のアクセサモジュールのための基底クラスを分離したかったので作りました

# Install

```shell
$ npm install github:umm/accessor_utility_monobehaviour.git
```

# Usage

```csharp
using UnityEngine;
using AccessorUtility;

public class Hoge : MonoBehaviour {

    private class Fuga {}

    public void Start() {
        Fuga fuga = new Fuga();
        this.PropertySet(fuga);
        Fuga fuga2 = this.PropertyGet<Fuga>();
        this.PropertyRemove<Fuga>();
    }

}
```

* 

# License

Copyright (c) 2017 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

