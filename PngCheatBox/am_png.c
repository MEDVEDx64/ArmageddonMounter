#include <stdint.h>
#include <stdbool.h>
#include <string.h>
#include <stdlib.h>
#include <png.h>

typedef struct {
	uint8_t* cur;
	uint8_t* end;
} stream_t;

typedef struct {
	void* pixels;
	void* palette;
	uint32_t palette_len;
	uint16_t width, height;
} img_data_t;

void create_stream(stream_t* s, void* data, int len) {
	s->cur = data;
	s->end = (uint8_t*)((int)data + len);
}

void read_data_from_mem(png_structp png_ptr, png_bytep dst, png_size_t count) {
	stream_t* s = (stream_t*)png_get_io_ptr(png_ptr);
	if (s == NULL) {
		png_error(png_ptr, "No stream pointer supplied");
		return;
	}

	if (((int)s->cur) + count >= (int)s->end) {
		png_error(png_ptr, "Stream read is too large");
		return;
	}

	memcpy(dst, s->cur, count);
	s->cur++;
}

#define CLEANUP() do { \
	png_destroy_info_struct(png_ptr, &info_ptr); \
	png_destroy_read_struct(&png_ptr, NULL, NULL); \
} while(0)

__declspec(dllexport) bool am_png_read(img_data_t* dst, void* src, int src_len) {
	memset(dst, 0, sizeof(img_data_t));

	if (!png_check_sig(src, 8))
		return false;

	png_structp png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL);
	if (!png_ptr)
		return false;

	png_infop info_ptr = png_create_info_struct(png_ptr);
	if (!info_ptr) {
		png_destroy_read_struct(&png_ptr, NULL, NULL);
		return false;
	}

	stream_t s;
	create_stream(&s, (void*)(((int)src) + 8), src_len);
	png_set_read_fn(png_ptr, &s, read_data_from_mem);

	png_set_sig_bytes(png_ptr, 8);
	png_read_info(png_ptr, info_ptr);

	int bit_depth = 0, color_type = 0;
	if (png_get_IHDR(png_ptr, info_ptr, &dst->width, &dst->height, &bit_depth, &color_type, NULL, NULL, NULL) != 1) {
		CLEANUP();
		return false;
	}

	if (bit_depth != 8 || color_type != PNG_COLOR_TYPE_PALETTE) {
		CLEANUP();
		return false;
	}

	dst->palette = malloc(0x100);
	png_get_PLTE(png_ptr, info_ptr, dst->palette, &dst->palette_len);
	dst->palette_len *= 3;

	dst->pixels = malloc(dst->width * dst->height);
	png_bytep row = dst->pixels;

	for (int i = 0; i < dst->height; i++) {
		png_read_row(png_ptr, row, NULL);
		row = (png_bytep)(((int)row) + dst->width);
	}

	CLEANUP();
	return true;
}

__declspec(dllexport) void am_png_read_end(img_data_t* data_ptr) {
	if(data_ptr->pixels)
		free(data_ptr->pixels);
	
	if(data_ptr->palette)
		free(data_ptr->palette);
}

__declspec(dllexport) int am_png_make(uint8_t* dst, int dst_len, uint8_t* src, int src_len, uint8_t* palette, int pal_len, int w, int h) {
	png_image img;
	memset(&img, 0, sizeof(img));

	img.version = PNG_IMAGE_VERSION;
	img.format = PNG_FORMAT_RGB_COLORMAP;
	img.width = w;
	img.height = h;
	img.colormap_entries = pal_len / 3;

	int wsize = dst_len;
	if (!png_image_write_to_memory(&img, dst, &wsize, 0, src, 0, palette))
		return 0;

	return wsize;
}