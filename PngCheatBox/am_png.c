#include <stdint.h>
#include <string.h>
#include <png.h>

__declspec(dllexport) int am_png_make(uint8_t* dst, int dst_len, uint8_t* src, int src_len, uint8_t* palette, int pal_len, int w, int h) {
	png_image img;
	memset(&img, 0, sizeof(img));

	img.version = PNG_IMAGE_VERSION;
	img.format = PNG_FORMAT_RGB_COLORMAP;
	img.width = w;
	img.height = h;
	img.colormap_entries = pal_len / 3;

	int wsize = dst_len;
	png_image_write_to_memory(&img, dst, &wsize, 0, src, 0, palette);

	return wsize;
}