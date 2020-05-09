// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Sprite.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LuxProtobuf {

  /// <summary>Holder for reflection information generated from Sprite.proto</summary>
  public static partial class SpriteReflection {

    #region Descriptor
    /// <summary>File descriptor for Sprite.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SpriteReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxTcHJpdGUucHJvdG8SC0x1eFByb3RvYnVmIlcKBlNwcml0ZRIMCgROYW1l",
            "GAEgASgJEhMKC1RleHR1cmVOYW1lGAIgASgJEioKCkFuaW1hdGlvbnMYAyAD",
            "KAsyFi5MdXhQcm90b2J1Zi5BbmltYXRpb24iRgoJQW5pbWF0aW9uEgwKBE5h",
            "bWUYASABKAkSKwoGRnJhbWVzGAIgAygLMhsuTHV4UHJvdG9idWYuQW5pbWF0",
            "aW9uRnJhbWUipAEKDkFuaW1hdGlvbkZyYW1lEg0KBVdpZHRoGAEgASgFEg4K",
            "BkhlaWdodBgCIAEoBRIYChBUZXh0dXJlUG9zaXRpb25YGAMgASgFEhgKEFRl",
            "eHR1cmVQb3NpdGlvblkYBCABKAUSLQoLU3ByaXRlRGVwdGgYBSABKA4yGC5M",
            "dXhQcm90b2J1Zi5TcHJpdGVEZXB0aBIQCghEdXJhdGlvbhgGIAEoBSplCgtT",
            "cHJpdGVEZXB0aBINCglVbmRlZmluZWQQABIHCgNNaW4QChIRCg1PdmVyQ2hh",
            "cmFjdGVyEB4SDQoJQ2hhcmFjdGVyECgSEwoPQmVoaW5kQ2hhcmFjdGVyEDIS",
            "BwoDTWF4EFpiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::LuxProtobuf.SpriteDepth), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::LuxProtobuf.Sprite), global::LuxProtobuf.Sprite.Parser, new[]{ "Name", "TextureName", "Animations" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::LuxProtobuf.Animation), global::LuxProtobuf.Animation.Parser, new[]{ "Name", "Frames" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::LuxProtobuf.AnimationFrame), global::LuxProtobuf.AnimationFrame.Parser, new[]{ "Width", "Height", "TexturePositionX", "TexturePositionY", "SpriteDepth", "Duration" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum SpriteDepth {
    [pbr::OriginalName("Undefined")] Undefined = 0,
    [pbr::OriginalName("Min")] Min = 10,
    [pbr::OriginalName("OverCharacter")] OverCharacter = 30,
    [pbr::OriginalName("Character")] Character = 40,
    [pbr::OriginalName("BehindCharacter")] BehindCharacter = 50,
    [pbr::OriginalName("Max")] Max = 90,
  }

  #endregion

  #region Messages
  public sealed partial class Sprite : pb::IMessage<Sprite> {
    private static readonly pb::MessageParser<Sprite> _parser = new pb::MessageParser<Sprite>(() => new Sprite());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Sprite> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LuxProtobuf.SpriteReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Sprite() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Sprite(Sprite other) : this() {
      name_ = other.name_;
      textureName_ = other.textureName_;
      animations_ = other.animations_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Sprite Clone() {
      return new Sprite(this);
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "TextureName" field.</summary>
    public const int TextureNameFieldNumber = 2;
    private string textureName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string TextureName {
      get { return textureName_; }
      set {
        textureName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Animations" field.</summary>
    public const int AnimationsFieldNumber = 3;
    private static readonly pb::FieldCodec<global::LuxProtobuf.Animation> _repeated_animations_codec
        = pb::FieldCodec.ForMessage(26, global::LuxProtobuf.Animation.Parser);
    private readonly pbc::RepeatedField<global::LuxProtobuf.Animation> animations_ = new pbc::RepeatedField<global::LuxProtobuf.Animation>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::LuxProtobuf.Animation> Animations {
      get { return animations_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Sprite);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Sprite other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (TextureName != other.TextureName) return false;
      if(!animations_.Equals(other.animations_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (TextureName.Length != 0) hash ^= TextureName.GetHashCode();
      hash ^= animations_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (TextureName.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(TextureName);
      }
      animations_.WriteTo(output, _repeated_animations_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (TextureName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(TextureName);
      }
      size += animations_.CalculateSize(_repeated_animations_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Sprite other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.TextureName.Length != 0) {
        TextureName = other.TextureName;
      }
      animations_.Add(other.animations_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 18: {
            TextureName = input.ReadString();
            break;
          }
          case 26: {
            animations_.AddEntriesFrom(input, _repeated_animations_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class Animation : pb::IMessage<Animation> {
    private static readonly pb::MessageParser<Animation> _parser = new pb::MessageParser<Animation>(() => new Animation());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Animation> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LuxProtobuf.SpriteReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Animation() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Animation(Animation other) : this() {
      name_ = other.name_;
      frames_ = other.frames_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Animation Clone() {
      return new Animation(this);
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Frames" field.</summary>
    public const int FramesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::LuxProtobuf.AnimationFrame> _repeated_frames_codec
        = pb::FieldCodec.ForMessage(18, global::LuxProtobuf.AnimationFrame.Parser);
    private readonly pbc::RepeatedField<global::LuxProtobuf.AnimationFrame> frames_ = new pbc::RepeatedField<global::LuxProtobuf.AnimationFrame>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::LuxProtobuf.AnimationFrame> Frames {
      get { return frames_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Animation);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Animation other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if(!frames_.Equals(other.frames_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      hash ^= frames_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      frames_.WriteTo(output, _repeated_frames_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      size += frames_.CalculateSize(_repeated_frames_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Animation other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      frames_.Add(other.frames_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 18: {
            frames_.AddEntriesFrom(input, _repeated_frames_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class AnimationFrame : pb::IMessage<AnimationFrame> {
    private static readonly pb::MessageParser<AnimationFrame> _parser = new pb::MessageParser<AnimationFrame>(() => new AnimationFrame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AnimationFrame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LuxProtobuf.SpriteReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AnimationFrame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AnimationFrame(AnimationFrame other) : this() {
      width_ = other.width_;
      height_ = other.height_;
      texturePositionX_ = other.texturePositionX_;
      texturePositionY_ = other.texturePositionY_;
      spriteDepth_ = other.spriteDepth_;
      duration_ = other.duration_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AnimationFrame Clone() {
      return new AnimationFrame(this);
    }

    /// <summary>Field number for the "Width" field.</summary>
    public const int WidthFieldNumber = 1;
    private int width_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Width {
      get { return width_; }
      set {
        width_ = value;
      }
    }

    /// <summary>Field number for the "Height" field.</summary>
    public const int HeightFieldNumber = 2;
    private int height_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Height {
      get { return height_; }
      set {
        height_ = value;
      }
    }

    /// <summary>Field number for the "TexturePositionX" field.</summary>
    public const int TexturePositionXFieldNumber = 3;
    private int texturePositionX_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int TexturePositionX {
      get { return texturePositionX_; }
      set {
        texturePositionX_ = value;
      }
    }

    /// <summary>Field number for the "TexturePositionY" field.</summary>
    public const int TexturePositionYFieldNumber = 4;
    private int texturePositionY_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int TexturePositionY {
      get { return texturePositionY_; }
      set {
        texturePositionY_ = value;
      }
    }

    /// <summary>Field number for the "SpriteDepth" field.</summary>
    public const int SpriteDepthFieldNumber = 5;
    private global::LuxProtobuf.SpriteDepth spriteDepth_ = global::LuxProtobuf.SpriteDepth.Undefined;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LuxProtobuf.SpriteDepth SpriteDepth {
      get { return spriteDepth_; }
      set {
        spriteDepth_ = value;
      }
    }

    /// <summary>Field number for the "Duration" field.</summary>
    public const int DurationFieldNumber = 6;
    private int duration_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Duration {
      get { return duration_; }
      set {
        duration_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AnimationFrame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AnimationFrame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Width != other.Width) return false;
      if (Height != other.Height) return false;
      if (TexturePositionX != other.TexturePositionX) return false;
      if (TexturePositionY != other.TexturePositionY) return false;
      if (SpriteDepth != other.SpriteDepth) return false;
      if (Duration != other.Duration) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Width != 0) hash ^= Width.GetHashCode();
      if (Height != 0) hash ^= Height.GetHashCode();
      if (TexturePositionX != 0) hash ^= TexturePositionX.GetHashCode();
      if (TexturePositionY != 0) hash ^= TexturePositionY.GetHashCode();
      if (SpriteDepth != global::LuxProtobuf.SpriteDepth.Undefined) hash ^= SpriteDepth.GetHashCode();
      if (Duration != 0) hash ^= Duration.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Width != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Width);
      }
      if (Height != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Height);
      }
      if (TexturePositionX != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(TexturePositionX);
      }
      if (TexturePositionY != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(TexturePositionY);
      }
      if (SpriteDepth != global::LuxProtobuf.SpriteDepth.Undefined) {
        output.WriteRawTag(40);
        output.WriteEnum((int) SpriteDepth);
      }
      if (Duration != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(Duration);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Width != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Width);
      }
      if (Height != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Height);
      }
      if (TexturePositionX != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(TexturePositionX);
      }
      if (TexturePositionY != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(TexturePositionY);
      }
      if (SpriteDepth != global::LuxProtobuf.SpriteDepth.Undefined) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) SpriteDepth);
      }
      if (Duration != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Duration);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AnimationFrame other) {
      if (other == null) {
        return;
      }
      if (other.Width != 0) {
        Width = other.Width;
      }
      if (other.Height != 0) {
        Height = other.Height;
      }
      if (other.TexturePositionX != 0) {
        TexturePositionX = other.TexturePositionX;
      }
      if (other.TexturePositionY != 0) {
        TexturePositionY = other.TexturePositionY;
      }
      if (other.SpriteDepth != global::LuxProtobuf.SpriteDepth.Undefined) {
        SpriteDepth = other.SpriteDepth;
      }
      if (other.Duration != 0) {
        Duration = other.Duration;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Width = input.ReadInt32();
            break;
          }
          case 16: {
            Height = input.ReadInt32();
            break;
          }
          case 24: {
            TexturePositionX = input.ReadInt32();
            break;
          }
          case 32: {
            TexturePositionY = input.ReadInt32();
            break;
          }
          case 40: {
            SpriteDepth = (global::LuxProtobuf.SpriteDepth) input.ReadEnum();
            break;
          }
          case 48: {
            Duration = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
