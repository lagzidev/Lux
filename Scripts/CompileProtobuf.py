import sys
import subprocess
import os

def main():
    if len(sys.argv) < 2:
        print("Usage: <script> <project_dir>")
        return

    project_dir = sys.argv[1]
    protoc_path = '{0}/../lib/protoc/bin/protoc'.format(project_dir)
    protoc_include_path = '{0}/../lib/protoc/include'.format(project_dir)

    # If Windows
    if sys.platform.startswith('win32'):
        protoc_path += '.exe'

    protobuf_files_path = '{0}/Protobuf'.format(project_dir)

    for filename in os.listdir(protobuf_files_path):
        if not filename.endswith('.proto'):
            continue

        filepath = '{0}/{1}'.format(protobuf_files_path, filename)
        result = subprocess.run([
            protoc_path, 
            '--proto_path=' + protobuf_files_path,
            '--proto_path=' + protoc_include_path,
            '--csharp_out=' + protobuf_files_path,
            filepath])

        # If failed, exit
        if result.returncode != 0:
            return result.returncode


if __name__ == "__main__":
    sys.exit(main())