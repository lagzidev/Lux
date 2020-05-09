import json
import sys
import subprocess
import os
from AsepriteExporter import AsepriteExporter
from CSProj import CSProj, CONTENT_DIR_NAME

TEXTURES_DIR_PATH = 'Textures'

def main():
	if (sys.argv.__len__() != 3):
		print("Usage: <script> <csproj_file_path> <aseprite_files_path>")
		return 1

	csproj_path = sys.argv[1]
	project_dir = os.path.dirname(csproj_path)
	content_dir = os.path.join(project_dir, CONTENT_DIR_NAME)
	textures_export_dir =  os.path.join(content_dir, TEXTURES_DIR_PATH)

	aseprite_files_path = sys.argv[2]

	aseprite_exporter = AsepriteExporter(textures_export_dir)
	aseprite_exporter.export(aseprite_files_path)

	csproj = CSProj(csproj_path)
	csproj.sync_content()

	return 0

if __name__ == '__main__':
	sys.exit(main())

