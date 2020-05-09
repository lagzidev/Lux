import os
import sys
import subprocess
import json

class AsepriteExporter:
	"""
	Exports aseprite files and imports them into the game
	"""
	def __init__(self, export_path):
		self.export_path = export_path

	def export(self, aseprite_files_path):
		for filename in os.listdir(aseprite_files_path):
			aseprite_file_path = '{0}/{1}'.format(aseprite_files_path, filename)

			aseprite_file = AsepriteFile(aseprite_file_path)
			if not aseprite_file.is_valid():
				continue

			# Export aseprite to png and get the json descriptor
			aseprite_json = aseprite_file.export(self.export_path)
			game_json = aseprite_json_to_game_json(aseprite_json)

			json_path = os.path.join(self.export_path, aseprite_file.get_filename_no_extension() + '.json')

			# If file already exists, get the existing json
			old_game_json = None
			if os.path.exists(json_path):
				with open(json_path, 'r') as f:
					try:
						# See if it's valid json
						old_game_json = json.load(f)
					except json.decoder.JSONDecodeError:
						print("Invalid existing json file, creating new")
						
			if old_game_json is not None:
				game_json = self._keep_modifications_to_game_json(game_json, old_game_json)

			# Create game json file in content directory
			with open(json_path, 'w') as f:
				json.dump(game_json, f)
				f.truncate()
	

	def _keep_modifications_to_game_json(self, new_game_json, old_game_json):
		returned_json = new_game_json

		# For each old animation
		for animation_key in old_game_json["Animations"]:
			# For each old frame
			for i in range(len(old_game_json["Animations"][animation_key]["Frames"])):
				old_frame = old_game_json["Animations"][animation_key]["Frames"][i]
				# Save these values
				returned_json["Animations"][animation_key]["Frames"][i]["Color"] = old_frame["Color"]
				returned_json["Animations"][animation_key]["Frames"][i]["Rotation"] = old_frame["Rotation"]
				returned_json["Animations"][animation_key]["Frames"][i]["Scale"] = old_frame["Scale"]
				returned_json["Animations"][animation_key]["Frames"][i]["SpriteEffects"] = old_frame["SpriteEffects"]
				returned_json["Animations"][animation_key]["Frames"][i]["SpriteDepth"] = old_frame["SpriteDepth"]
				returned_json["Animations"][animation_key]["Frames"][i]["Duration"] = old_frame["Duration"]

		return returned_json


	@staticmethod
	def get_program_path():
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



class AsepriteFile:
	def __init__(self, path):
		#super().__init__()
		self.path = path.replace('\\', '/')
		self.filename_no_extension = path.rpartition('/')[2].rpartition('.')[0]


	def is_valid(self):
		return self.path.endswith('.ase') or self.path.endswith('.aseprite')


	def export(self, dest_png_dir, dest_json_dir=None):
		temp = ''
		if dest_json_dir is None:
			dest_json_dir = dest_png_dir
			temp = '_pipeline_temp'

		png_path = os.path.join(dest_png_dir, self.filename_no_extension + '.png')
		json_path = os.path.join(dest_json_dir, '{0}{1}.json'.format(self.filename_no_extension, temp))

		result = subprocess.run([
			AsepriteExporter.get_program_path(), 
			'-b', self.path, 
			'--sheet', png_path, 
			'--data', json_path, 
			'--list-tags', 
			'--shape-padding', '1',
			'--border-padding', '1',
			'--sheet-width', '2048',
		])

		if result.returncode != 0:
			raise Exception("Aseprite failed to export")

		aseprite_json = AsepriteJSON(json_path)

		if temp:
			os.remove(json_path)

		return aseprite_json


	def get_filename_no_extension(self):
		return self.filename_no_extension


def aseprite_json_to_game_json(aseprite_json):
	json_result = {
		"Animations": {}
	}

	# For each frame tag
	for current_tag in aseprite_json.get_frametags():
		json_result['Animations'][current_tag['name']] = {
			"Frames": []
		}

		# For each relevent frame
		for current_frame in aseprite_json.get_frames_by_frametag(current_tag):
			json_result['Animations'][current_tag['name']]['Frames'].append({
				"Width": current_frame['frame']['w'],
				"Height": current_frame['frame']['h'],
				"TexturePositionX": current_frame['frame']['x'],
				"TexturePositionY": current_frame['frame']['y'],
				"Color": "255,255,255,255",
				"Rotation": 0,
				"Scale": "1,1",
				"SpriteEffects": 0,
				"SpriteDepth": 5,
				"Duration": current_frame['duration']
			})


	return json_result
