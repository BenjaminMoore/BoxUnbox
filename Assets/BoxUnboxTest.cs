using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public enum TestCase {
    Simple      = 0,
    Convertible = 1,
    Unsafe      = 2
}

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public class BoxUnboxTest : MonoBehaviour {
    public Text stateText;

    private TestCase state;
    private int      temp;

    public void SetState(int newState) {
        this.state = (TestCase) newState;

        this.stateText.text = this.state.ToString();
    }

    public void Awake() {
        this.SetState(0);
    }

    public void Update() {
        switch (this.state) {
            case TestCase.Simple:
                for (var i = 0; i < 1_000_000; i++) {
                    this.temp = this.SimpleCase(i);
                }

                break;
            case TestCase.Convertible:
                for (var i = 0; i < 1_000_000; i++) {
                    this.temp = this.ConvertibleCase(i);
                }

                break;
            case TestCase.Unsafe:
                for (var i = 0; i < 1_000_000; i++) {
                    this.temp = this.UnsafeCase(i);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SimpleCase<T>(T someValue) {
        if (typeof(T) == typeof(int)) {
            var poher = (int) (object) someValue;
            return (T) (object) ++poher;
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ConvertibleCase<T>(T someValue) where T : IConvertible {
        if (typeof(T) == typeof(int)) {
            var poher = someValue.ToInt32(CultureInfo.InvariantCulture);
            return (T) (object) ++poher;
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UnsafeCase<T>(T someValue) {
        if (typeof(T) == typeof(int)) {
            ref var poher = ref Unsafe.As<T, int>(ref someValue);
            poher++;
            return Unsafe.As<int, T>(ref poher);
        }

        return default;
    }
}

namespace Unity.IL2CPP.CompilerServices {
    using System;

    public enum Option {
        NullChecks         = 1,
        ArrayBoundsChecks  = 2,
        DivideByZeroChecks = 3
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class Il2CppSetOptionAttribute : Attribute {
        public Option Option { get; }
        public object Value  { get; }

        public Il2CppSetOptionAttribute(Option option, object value) {
            this.Option = option;
            this.Value  = value;
        }
    }
}