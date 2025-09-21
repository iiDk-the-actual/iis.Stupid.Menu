import os

# Root folder where your .cs files are located
ROOT_DIR = "G:/Other Stuff/Gorilla Tag Mods/! ii'sStupidMenu"

# The license header template
HEADER_TEMPLATE = """/*
 * ii's Stupid Menu  {path}
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
"""

def prepend_header_to_cs_files(root_dir):
    for dirpath, _, filenames in os.walk(root_dir):
        for filename in filenames:
            if filename.endswith(".cs"):
                file_path = os.path.join(dirpath, filename)
                rel_path = os.path.relpath(file_path, root_dir).replace("\\", "/")

                # Read existing file content
                with open(file_path, "r", encoding="utf-8") as f:
                    content = f.read()

                # Check if header is already there
                if "ii's Stupid Menu" in content.splitlines()[0:5]:
                    print(f"Skipping (already has header): {rel_path}")
                    continue

                # Build header
                header = HEADER_TEMPLATE.format(path=rel_path)

                # Write header + old content
                with open(file_path, "w", encoding="utf-8") as f:
                    f.write(header + "\n" + content)

                print(f"Updated: {rel_path}")

if __name__ == "__main__":
    prepend_header_to_cs_files(ROOT_DIR)
