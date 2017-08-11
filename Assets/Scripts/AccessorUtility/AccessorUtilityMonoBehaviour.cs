using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace AccessorUtility {

    /// <summary>
    /// MonoBehaviour のアクセサ用クラス
    /// </summary>
    public class AccessorUtilityMonoBehaviour : AccessorUtility<MonoBehaviour, Component, AccessorUtilityMonoBehaviour.ReferenceMap<MonoBehaviour, Component, AccessorUtilityMonoBehaviour.InstanceMap<Component>>, AccessorUtilityMonoBehaviour.InstanceMap<Component>> {

        /// <summary>
        /// MonoBehaviour のインスタンスをキーにした参照ディクショナリ
        /// </summary>
        /// <remarks>弱参照っぽい実装を Getter 内に施している</remarks>
        /// <typeparam name="TKey">MonoBehaviour</typeparam>
        /// <typeparam name="TValue">インスタンス格納用のディクショナリ</typeparam>
        /// <typeparam name="TInstanceMap">インスタンス保存用のディクショナリの型</typeparam>
        public class ReferenceMap<TKey, TValue, TInstanceMap> : BasicReferenceMap<TKey, TValue, TInstanceMap> where TKey : MonoBehaviour where TValue : Component where TInstanceMap : InstanceMap<TValue>, new() {

            public new TInstanceMap this[TKey key] {
                get {
                    TInstanceMap value;
                    if (!this.TryGetValue(key, out value)) {
                        value = new TInstanceMap();
                        // 弱参照もどきとして、キーとなっている MonoBehaviour が破棄されるときに管理ディクショナリから消す
                        key.OnDestroyAsObservable().Subscribe(_ => instanceMapMap.Remove(key));
                        base[key] = value;
                    }
                    return value;
                }
                set {
                    base[key] = value;
                }
            }

        }

        /// <summary>
        /// Getter を拡張したインスタンス保存用のディクショナリ
        /// </summary>
        /// <remarks>KeyNotFoundException を防いでいる</remarks>
        /// <typeparam name="TValue">値の型</typeparam>
        public class InstanceMap<TValue> : BasicInstanceMap<TValue> where TValue : Component {

            public new TValue this[Type key] {
                get {
                    TValue value;
                    this.TryGetValue(key, out value);
                    return value;
                }
                set {
                    base[key] = value;
                }
            }

        }

    }

    /// <summary>
    /// MonoBehaviour にアクセサもどきを提供する拡張クラス
    /// </summary>
    public static class MonoBehaviourAccessorUtilityExtension {

        /// <summary>
        /// プロパティが存在するかどうかを調べる
        /// </summary>
        /// <remarks>デフォルト値 (null) の場合存在しないモノとする</remarks>
        /// <param name="self">MonoBehaviour インスタンス</param>
        /// <typeparam name="T">値の型</typeparam>
        /// <returns>存在する場合真</returns>
        public static bool PropertyExists<T>(this MonoBehaviour self) where T : Component {
            return (self.PropertyGet<T>() != default(T));
        }

        /// <summary>
        /// プロパティを取得
        /// </summary>
        /// <param name="self">MonoBehaviour インスタンス</param>
        /// <typeparam name="T">値の型</typeparam>
        /// <returns>プロパティの値</returns>
        public static T PropertyGet<T>(this MonoBehaviour self) where T : Component {
            return AccessorUtilityMonoBehaviour.instanceMapMap[self][typeof(T)] as T;
        }

        /// <summary>
        /// プロパティを管理ディクショナリに設定
        /// </summary>
        /// <param name="self">MonoBehaviour インスタンス</param>
        /// <param name="value">セットする値</param>
        /// <typeparam name="T">値の型</typeparam>
        public static void PropertySet<T>(this MonoBehaviour self, T value) where T : Component {
            if (!self.PropertyExists<T>() && value != null) {
                // 弱参照もどきとして、コンポーネントが破棄されるときに管理ディクショナリから消す
                value.OnDestroyAsObservable().Subscribe(_ => self.PropertyRemove<T>());
            }
            AccessorUtilityMonoBehaviour.instanceMapMap[self][typeof(T)] = value;
        }

        /// <summary>
        /// プロパティを管理ディクショナリから削除
        /// </summary>
        /// <param name="self">MonoBehaviour インスタンス</param>
        /// <typeparam name="T">値の型</typeparam>
        public static void PropertyRemove<T>(this MonoBehaviour self) where T : Component {
            AccessorUtilityMonoBehaviour.instanceMapMap[self].Remove(typeof(T));
        }

    }

}