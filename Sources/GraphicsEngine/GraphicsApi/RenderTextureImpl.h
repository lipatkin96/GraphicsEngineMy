#ifndef RENDERTEXTUREIMPL_H
#define RENDERTEXTUREIMPL_H

enum RenderTarget {
    SCREEN,
    DEPTH_FLAT_TEXTURE,
    DEPTH_CUBEMAP_TEXTURE
};

class RenderTextureImpl
{
public:
    virtual void Init() = 0;
    virtual void Deinit() = 0;

    virtual void SetRenderTarget(RenderTarget location) = 0;

    virtual RenderTarget GetRenderTarget() const = 0;
};

#endif // RENDERTEXTURE_H
