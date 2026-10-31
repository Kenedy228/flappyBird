// Verify the actual Make -> dotnet run -> Program startup, independently of build.
// WindowServer observation confirms a native window, not pixels or gameplay input.
import AppKit
import CoreGraphics
import Foundation

let logURL = FileManager.default.temporaryDirectory.appendingPathComponent(UUID().uuidString)
FileManager.default.createFile(atPath: logURL.path, contents: nil)
let log = try FileHandle(forWritingTo: logURL)
let make = Process()
make.executableURL = URL(fileURLWithPath: CommandLine.arguments[1])
make.arguments = ["run"]
make.standardOutput = log
make.standardError = log

func fail(_ message: String) throws {
    throw NSError(domain: "Make startup", code: 1,
                  userInfo: [NSLocalizedDescriptionKey: message])
}

func ownedWindow() -> (NSRunningApplication, [String: Any])? {
    let expected = URL(fileURLWithPath: FileManager.default.currentDirectoryPath)
        .appendingPathComponent("FlappyBird/bin/Release/net10.0/FlappyBird")
        .resolvingSymlinksInPath()
    let windows = CGWindowListCopyWindowInfo(.optionOnScreenOnly, kCGNullWindowID)
        as? [[String: Any]] ?? []
    for window in windows {
        guard let pid = window[kCGWindowOwnerPID as String] as? Int32,
              let app = NSRunningApplication(processIdentifier: pid),
              app.executableURL?.resolvingSymlinksInPath() == expected,
              let layer = window[kCGWindowLayer as String] as? Int, layer == 0
        else { continue }
        return (app, window)
    }
    return nil
}

var result: Int32 = 0
do {
    try make.run()
    let deadline = Date().addingTimeInterval(60)
    var found: (NSRunningApplication, [String: Any])?
    while Date() < deadline && make.isRunning {
        if let window = ownedWindow() { found = window; break }
        Thread.sleep(forTimeInterval: 0.2)
    }
    guard let (app, window) = found else {
        try fail("make run did not create its on-screen FlappyBird window")
        fatalError("unreachable")
    }
    print("WindowServer observed application PID \(app.processIdentifier), bounds \(window[kCGWindowBounds as String] ?? "unknown")")
    Thread.sleep(forTimeInterval: 2)
    if !make.isRunning { try fail("Application exited after opening its window") }
    if !app.terminate() { try fail("Application rejected a normal Quit request") }
    let closeDeadline = Date().addingTimeInterval(15)
    while make.isRunning && Date() < closeDeadline { Thread.sleep(forTimeInterval: 0.1) }
    if make.isRunning { try fail("make run did not finish after Quit") }
    if make.terminationStatus != 0 { try fail("make run failed: \(make.terminationStatus)") }
    print("PASS: actual gmake run opened an on-screen window and exited 0 after Quit")
} catch {
    fputs("\(error.localizedDescription)\n", stderr)
    result = 1
}
if make.isRunning {
    let cleanup = Process()
    cleanup.executableURL = URL(fileURLWithPath: "/usr/bin/pkill")
    cleanup.arguments = ["-TERM", "-P", String(make.processIdentifier)]
    try? cleanup.run()
    cleanup.waitUntilExit()
    make.terminate()
    make.waitUntilExit()
}
try log.close()
print(try String(contentsOf: logURL, encoding: .utf8))
try FileManager.default.removeItem(at: logURL)
exit(result)
