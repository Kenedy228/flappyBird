"""Check Make's error propagation at the real SDK command boundary.

The shim fails one selected SDK operation and forwards all other operations to
the installed SDK. A marker distinguishes the intended failure from a bad path
or shell error. This also exercises a DOTNET override containing spaces.
"""

import argparse
import hashlib
import os
from pathlib import Path
import shlex
import shutil
import subprocess
import tempfile


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--make", default="make")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        raise SystemExit("dotnet SDK is required")

    with tempfile.TemporaryDirectory(prefix="make SDK failure ") as directory:
        shim = Path(directory) / ("SDK.cmd" if os.name == "nt" else "SDK")
        for target, operation in (
            ("restore", "restore"),
            ("build", "restore"),
            ("build", "build"),
            ("test", "test"),
            ("check", "test"),
            ("run", "run"),
            ("clean", "clean"),
        ):
            marker = f"intentional SDK failure: {operation}"
            if os.name == "nt":
                shim.write_text(
                    f'@echo off\nif "%~1"=="{operation}" (\n'
                    f"echo {marker}\nexit /b 37\n)\n"
                    f'"{dotnet}" %*\nexit /b %errorlevel%\n'
                )
            else:
                shim.write_text(
                    f'#!/bin/sh\nif [ "$1" = "{operation}" ]; then\n'
                    f"echo '{marker}'\nexit 37\nfi\n"
                    f'exec {shlex.quote(dotnet)} "$@"\n'
                )
                shim.chmod(0o700)
            result = subprocess.run(
                [args.make, target, f"DOTNET={shim}"],
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                text=True,
                timeout=120,
            )
            if result.returncode == 0 or marker not in result.stdout:
                raise AssertionError(
                    f"{target}: expected propagated {operation} failure, "
                    f"exit={result.returncode}\n{result.stdout}"
                )
            print(f"PASS: make {target} rejects SDK {operation} failure", flush=True)

    tracked = subprocess.check_output(["git", "ls-files", "-z"]).split(b"\0")
    paths = [Path(os.fsdecode(path)) for path in tracked if path]
    before = {path: hashlib.sha256(path.read_bytes()).digest() for path in paths}
    # Cleaning must preserve sources, textures, lockfiles and project settings.
    subprocess.run([args.make, "clean"], check=True, timeout=120)
    for path, digest in before.items():
        if not path.is_file() or hashlib.sha256(path.read_bytes()).digest() != digest:
            raise AssertionError(f"make clean changed a tracked file: {path}")
    print(f"PASS: make clean preserves all {len(before)} tracked files", flush=True)


if __name__ == "__main__":
    main()
