import os

def list_large_files(folder_path):
    large_files = []
    for dirpath, dirnames, filenames in os.walk(folder_path):
        for filename in filenames:
            file_path = os.path.join(dirpath, filename)
            if os.path.isfile(file_path) and os.path.getsize(file_path) > 100 * 1024 * 1024:  # 100MB
                large_files.append(file_path)
    return large_files

folder_path = '.'
large_files = list_large_files(folder_path)

for file in large_files:
    print(file)