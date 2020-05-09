# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Sprite.proto

from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='Sprite.proto',
  package='LuxProtobuf',
  syntax='proto3',
  serialized_options=None,
  serialized_pb=b'\n\x0cSprite.proto\x12\x0bLuxProtobuf\"W\n\x06Sprite\x12\x0c\n\x04Name\x18\x01 \x01(\t\x12\x13\n\x0bTextureName\x18\x02 \x01(\t\x12*\n\nAnimations\x18\x03 \x03(\x0b\x32\x16.LuxProtobuf.Animation\"F\n\tAnimation\x12\x0c\n\x04Name\x18\x01 \x01(\t\x12+\n\x06\x46rames\x18\x02 \x03(\x0b\x32\x1b.LuxProtobuf.AnimationFrame\"\xa4\x01\n\x0e\x41nimationFrame\x12\r\n\x05Width\x18\x01 \x01(\x05\x12\x0e\n\x06Height\x18\x02 \x01(\x05\x12\x18\n\x10TexturePositionX\x18\x03 \x01(\x05\x12\x18\n\x10TexturePositionY\x18\x04 \x01(\x05\x12-\n\x0bSpriteDepth\x18\x05 \x01(\x0e\x32\x18.LuxProtobuf.SpriteDepth\x12\x10\n\x08\x44uration\x18\x06 \x01(\x05*e\n\x0bSpriteDepth\x12\r\n\tUndefined\x10\x00\x12\x07\n\x03Min\x10\n\x12\x11\n\rOverCharacter\x10\x1e\x12\r\n\tCharacter\x10(\x12\x13\n\x0f\x42\x65hindCharacter\x10\x32\x12\x07\n\x03Max\x10Zb\x06proto3'
)

_SPRITEDEPTH = _descriptor.EnumDescriptor(
  name='SpriteDepth',
  full_name='LuxProtobuf.SpriteDepth',
  filename=None,
  file=DESCRIPTOR,
  values=[
    _descriptor.EnumValueDescriptor(
      name='Undefined', index=0, number=0,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Min', index=1, number=10,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='OverCharacter', index=2, number=30,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Character', index=3, number=40,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='BehindCharacter', index=4, number=50,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Max', index=5, number=90,
      serialized_options=None,
      type=None),
  ],
  containing_type=None,
  serialized_options=None,
  serialized_start=357,
  serialized_end=458,
)
_sym_db.RegisterEnumDescriptor(_SPRITEDEPTH)

SpriteDepth = enum_type_wrapper.EnumTypeWrapper(_SPRITEDEPTH)
Undefined = 0
Min = 10
OverCharacter = 30
Character = 40
BehindCharacter = 50
Max = 90



_SPRITE = _descriptor.Descriptor(
  name='Sprite',
  full_name='LuxProtobuf.Sprite',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='Name', full_name='LuxProtobuf.Sprite.Name', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='TextureName', full_name='LuxProtobuf.Sprite.TextureName', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='Animations', full_name='LuxProtobuf.Sprite.Animations', index=2,
      number=3, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=29,
  serialized_end=116,
)


_ANIMATION = _descriptor.Descriptor(
  name='Animation',
  full_name='LuxProtobuf.Animation',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='Name', full_name='LuxProtobuf.Animation.Name', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='Frames', full_name='LuxProtobuf.Animation.Frames', index=1,
      number=2, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=118,
  serialized_end=188,
)


_ANIMATIONFRAME = _descriptor.Descriptor(
  name='AnimationFrame',
  full_name='LuxProtobuf.AnimationFrame',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='Width', full_name='LuxProtobuf.AnimationFrame.Width', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='Height', full_name='LuxProtobuf.AnimationFrame.Height', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='TexturePositionX', full_name='LuxProtobuf.AnimationFrame.TexturePositionX', index=2,
      number=3, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='TexturePositionY', full_name='LuxProtobuf.AnimationFrame.TexturePositionY', index=3,
      number=4, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='SpriteDepth', full_name='LuxProtobuf.AnimationFrame.SpriteDepth', index=4,
      number=5, type=14, cpp_type=8, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='Duration', full_name='LuxProtobuf.AnimationFrame.Duration', index=5,
      number=6, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=191,
  serialized_end=355,
)

_SPRITE.fields_by_name['Animations'].message_type = _ANIMATION
_ANIMATION.fields_by_name['Frames'].message_type = _ANIMATIONFRAME
_ANIMATIONFRAME.fields_by_name['SpriteDepth'].enum_type = _SPRITEDEPTH
DESCRIPTOR.message_types_by_name['Sprite'] = _SPRITE
DESCRIPTOR.message_types_by_name['Animation'] = _ANIMATION
DESCRIPTOR.message_types_by_name['AnimationFrame'] = _ANIMATIONFRAME
DESCRIPTOR.enum_types_by_name['SpriteDepth'] = _SPRITEDEPTH
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

Sprite = _reflection.GeneratedProtocolMessageType('Sprite', (_message.Message,), {
  'DESCRIPTOR' : _SPRITE,
  '__module__' : 'Sprite_pb2'
  # @@protoc_insertion_point(class_scope:LuxProtobuf.Sprite)
  })
_sym_db.RegisterMessage(Sprite)

Animation = _reflection.GeneratedProtocolMessageType('Animation', (_message.Message,), {
  'DESCRIPTOR' : _ANIMATION,
  '__module__' : 'Sprite_pb2'
  # @@protoc_insertion_point(class_scope:LuxProtobuf.Animation)
  })
_sym_db.RegisterMessage(Animation)

AnimationFrame = _reflection.GeneratedProtocolMessageType('AnimationFrame', (_message.Message,), {
  'DESCRIPTOR' : _ANIMATIONFRAME,
  '__module__' : 'Sprite_pb2'
  # @@protoc_insertion_point(class_scope:LuxProtobuf.AnimationFrame)
  })
_sym_db.RegisterMessage(AnimationFrame)


# @@protoc_insertion_point(module_scope)
