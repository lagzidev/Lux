import os
import sys
import subprocess
import json
from Protobuf import Sprite_pb2
from google.protobuf.json_format import MessageToJson


class ContentFileHandler(object):
	SUPPORTED_EXTENSIONS = []

	def __init__(self, root_input_dir, root_output_dir):
		self.root_input_dir = root_input_dir
		self.root_output_dir = root_output_dir


	def _handle_file(self, input_filepath, output_filepath):
		raise NotImplementedError()

	
	@staticmethod
	def change_extension(filepath, new_extension):
		return os.path.splitext(filepath)[0] + new_extension


	@staticmethod
	def get_filename(filepath):
		base = os.path.basename(filepath)
		return os.path.splitext(base)[0]

	def handle(self, filepath):
		(_, extension) = os.path.splitext(filepath)
		if extension not in self.SUPPORTED_EXTENSIONS:
			return

		relative_filepath = os.path.relpath(filepath, self.root_input_dir)
		mirrored_content_filepath = os.path.join(self.root_output_dir, relative_filepath)

		self._handle_file(filepath, mirrored_content_filepath)


class AsepriteHandler(ContentFileHandler):
	SUPPORTED_EXTENSIONS = ['.aseprite', '.ase']

	def __init__(self, root_input_dir, root_output_dir):
		ContentFileHandler.__init__(self, root_input_dir, root_output_dir)

	def _handle_file(self, input_filepath, output_filepath):
		# Export aseprite to png and get the json descriptor
		png_output_path = ContentFileHandler.change_extension(output_filepath, '.png')
		aseprite_json = self._export_aseprite(input_filepath, png_output_path)

		# TODO: Add the texture to an atlas and provide the atlas' image path instead of png_path
		filename = ContentFileHandler.get_filename(output_filepath)
		game_json = self._aseprite_json_to_game_json(filename, aseprite_json)

		json_output_path = ContentFileHandler.change_extension(output_filepath, '.json')

		# Create game json file in content directory
		with open(json_output_path, 'w') as f:
			json.dump(game_json, f)
			f.truncate()


	def _export_aseprite(self, aseprite_filepath, dest_png_path, dest_json_path=None):
		temp = False
		if dest_json_path is None:
			temp = True
			dest_json_path = ContentFileHandler.change_extension(dest_png_path, '.tmp.json')

		result = subprocess.run([
			self._get_program_path(),
			'-b', aseprite_filepath, 
			'--sheet', dest_png_path, 
			'--data', dest_json_path, 
			'--list-tags', 
			'--shape-padding', '1',
			'--border-padding', '1',
			'--sheet-width', '2048',
		])

		if result.returncode != 0:
			raise Exception("Aseprite failed to export")

		aseprite_json = AsepriteJSON(dest_json_path)

		if temp:
			os.remove(dest_json_path)

		return aseprite_json


	def _aseprite_json_to_game_json(self, texture_name, aseprite_json):
		sprite = Sprite_pb2.Sprite()
		sprite.TextureName = texture_name

		# For each frame tag
		frametags = aseprite_json.get_frametags()

		if len(frametags) > 0:
			sprite.DefaultAnimationName = frametags[0]['name']

		for current_tag in frametags:
			frames = []

			# Construct frames
			for current_frame in aseprite_json.get_frames_by_frametag(current_tag):
				frames.append(Sprite_pb2.AnimationFrame(
					Width = current_frame['frame']['w'],
					Height = current_frame['frame']['h'],
					TexturePositionX = current_frame['frame']['x'],
					TexturePositionY = current_frame['frame']['y'],
					SpriteDepth = Sprite_pb2.SpriteDepth.BehindCharacter,
					Duration = current_frame['duration']
				))

			# Add animation to sprite
			sprite.Animations[current_tag['name']].Frames.extend(frames)


		json_str = MessageToJson(sprite, preserving_proto_field_name=True)

		return json.loads(json_str)


	def _get_program_path(self):
		program_path = r"C:\Program Files (x86)\Aseprite\Aseprite.exe"

		# If Windows
		if sys.platform.startswith('win32'):
			if not os.path.isfile(program_path):
				program_path = r"C:\Program Files\Aseprite\Aseprite.exe"

		# If MacOS
		if sys.platform.startswith('darwin'):
			program_path = '/Applications/Aseprite.app/Contents/MacOS/aseprite'

		return program_path


class AsepriteJSON:
	def __init__(self, path):
		if not path.endswith('.json'):
			raise Exception('AsepriteJSON got a non json file')

		self.json = None
		with open(path, 'r') as f:
			self.json = json.load(f)


	def get_frametags(self):
		return self.json['meta']['frameTags']


	def get_frames_by_frametag(self, frametag):
		frames = []
		for i in range(frametag['from'], frametag['to'] + 1):
			frames.append(self._get_frame_by_index(i))

		return frames


	# TODO: Make this more reliable by looking for {i}.asperite
	# instead of by index which is unreliable for json objects
	def _get_frame_by_index(self, requestedIndex):
		i = 0
		for frameName in self.json['frames']:
			if requestedIndex == i:
				return self.json['frames'][frameName]
			i += 1