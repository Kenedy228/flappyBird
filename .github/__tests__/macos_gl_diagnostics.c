/* Diagnostic evidence for failed startup; this is not a passing graphics test. */
#include <OpenGL/OpenGL.h>
#include <stdio.h>

static void describe_format(const char *name, const CGLPixelFormatAttribute *attributes)
{
    CGLPixelFormatObj format = NULL;
    GLint count = 0;
    CGLError error = CGLChoosePixelFormat(attributes, &format, &count);
    printf("CGL format %s: error=%d (%s), count=%d, allocated=%s\n",
           name, (int)error, CGLErrorString(error), (int)count,
           format ? "yes" : "no");
    if (format)
        CGLDestroyPixelFormat(format);
}

int main(void)
{
    CGLRendererInfoObj info = NULL;
    GLint count = 0;
    CGLError error = CGLQueryRendererInfo(0xffffffff, &info, &count);
    printf("CGL renderers: error=%d (%s), count=%d\n",
           (int)error, CGLErrorString(error), (int)count);
    if (info) {
        for (GLint i = 0; i < count; ++i) {
            GLint renderer = 0, accelerated = 0;
            CGLError id_error = CGLDescribeRenderer(info, i, kCGLRPRendererID, &renderer);
            CGLError acceleration_error = CGLDescribeRenderer(info, i, kCGLRPAccelerated, &accelerated);
            printf("CGL renderer %d: id=0x%x (error=%d), accelerated=%d (error=%d)\n",
                   (int)i, (unsigned)renderer, (int)id_error,
                   (int)accelerated, (int)acceleration_error);
        }
        CGLDestroyRendererInfo(info);
    }

    const CGLPixelFormatAttribute minimal[] = {0};
    const CGLPixelFormatAttribute accelerated[] = {kCGLPFAAccelerated, 0};
    const CGLPixelFormatAttribute window[] = {
        kCGLPFAClosestPolicy, kCGLPFADoubleBuffer,
        kCGLPFAColorSize, 24, kCGLPFAAlphaSize, 8,
        kCGLPFADepthSize, 24, kCGLPFAStencilSize, 8, 0
    };
    const CGLPixelFormatAttribute accelerated_window[] = {
        kCGLPFAAccelerated, kCGLPFAClosestPolicy, kCGLPFADoubleBuffer,
        kCGLPFAColorSize, 24, kCGLPFAAlphaSize, 8,
        kCGLPFADepthSize, 24, kCGLPFAStencilSize, 8, 0
    };
    describe_format("minimal", minimal);
    describe_format("accelerated minimal", accelerated);
    describe_format("window without acceleration constraint", window);
    describe_format("accelerated window", accelerated_window);
    return 0;
}
