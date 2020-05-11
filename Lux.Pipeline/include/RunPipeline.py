import json
import sys
import subprocess
import os
from AsepriteExporter import AsepriteHandler
from CSProj import CSProj, CONTENT_DIR_NAME

class PipelineFileObject(object):
	def __init__(self, filepath):
		self.filepath = filepath

	def run(self, callback, *args):
		raise NotImplementedError()


class PipelineFile(PipelineFileObject):
	def __init__(self, filepath):
		PipelineFileObject.__init__(self, filepath)

	def run(self, callback, *args):
		callback(self.filepath, *args)

class PipelineDir(PipelineFileObject):
	def __init__(self, filepath):
		PipelineFileObject.__init__(self, filepath)

	def run(self, callback, *args):
		for filename in os.listdir(self.filepath):
			child_path = os.path.join(self.filepath, filename)

			# Create a fileobject as a file or a dir
			fileobject = None
			if os.path.isfile(child_path):
				fileobject = PipelineFile(child_path)
			else:
				fileobject = PipelineDir(child_path)

			fileobject.run(callback, *args)


def handle_file(filepath, handlers):
	for handler in handlers:
		handler.handle(filepath)

def main():
	if (sys.argv.__len__() != 3):
		print("Usage: <script> <csproj_file_path> <pipeline_files_dir>")
		return 1

	csproj_path = sys.argv[1]
	project_dir = os.path.dirname(csproj_path)
	content_dir = os.path.join(project_dir, CONTENT_DIR_NAME)

	pipeline_files_dir_path = sys.argv[2]

	# Create file handlers
	handlers = [
		AsepriteHandler(pipeline_files_dir_path, content_dir)
	]

	# Run handle_file for every file in pipeline files dir
	files_dir = PipelineDir(pipeline_files_dir_path)
	files_dir.run(handle_file, handlers)

	csproj = CSProj(csproj_path)
	csproj.sync_content()

	return 0

if __name__ == '__main__':
	sys.exit(main())

