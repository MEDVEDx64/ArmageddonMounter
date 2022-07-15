#include "stdafx.h"

#define HTAB_SIZE 0x400
#define WRITE_BLK 0x100000
#define TOC_OFFSET_BASE 0x1004

#define HASH_BITS 10
#define HASH_SIZE (1 << HASH_BITS)

uint32_t htab[HTAB_SIZE];
uint8_t* ftab = NULL;
int ftab_size = 0;
FILE* dir_f = NULL;

typedef struct dir_file_entry {
	uint32_t next_entry;
	uint32_t file_offset;
	uint32_t file_length;

	char name[0];
} dir_file_entry_t;

static int hash(char* str) {
	int sum = 0;
	while (*str) {
		sum = ((sum << 1) % HASH_SIZE) | (sum >> (HASH_BITS - 1) & 1);
		sum += (unsigned char)*str++;
		sum %= HASH_SIZE;
	}

	return sum;
}

bool write_data(void* data, int len) {
	uintptr_t data_ptr = (uintptr_t)data;
	int blk_len;

	while (len) {
		blk_len = len > WRITE_BLK ? WRITE_BLK : len;
		if (fwrite((void*)data_ptr, blk_len, 1, dir_f) != 1)
			return false;

		data_ptr += blk_len;
		len -= blk_len;
	}

	return true;
}

__declspec(dllexport) bool am_dir_begin(const wchar_t* path) {
	memset(htab, 0, HTAB_SIZE * 4);
	ftab_size = 0;
	_wfopen_s(&dir_f, path, L"wb");
	if (!dir_f)
		return false;

	uint32_t header[3] = { 0x1a524944, 0, 0 };
	if (fwrite(header, 12, 1, dir_f) != 1)
		return false;

	return true;
}

__declspec(dllexport) bool am_dir_add(void* data, int len, const char* name) {
	uint32_t offset = (uint32_t)ftell(dir_f);
	int name_len = strlen(name);
	int entry_hash = hash(name);

	if (htab[entry_hash])
		return false; // duplicate file found (or a collision)

	if (!write_data(data, len))
		return false;

	htab[entry_hash] = TOC_OFFSET_BASE + ftab_size;

	int entry_size = 12 /* entry struct size */ + name_len + (4 - name_len % 4) /* align */;

	if (ftab)
		ftab = realloc(ftab, ftab_size + entry_size);
	else
		ftab = malloc(entry_size);

	dir_file_entry_t* entry = (dir_file_entry_t*)(((uintptr_t)ftab) + ftab_size);
	memset(entry, 0, entry_size);
	
	entry->file_offset = offset;
	entry->file_length = len;
	memcpy(entry->name, name, name_len);

	ftab_size += entry_size;
	return true;
}

__declspec(dllexport) bool am_dir_end() {
	int toc_offset = ftell(dir_f);
	uint32_t magic = 0xa;

	if (fwrite(&magic, 4, 1, dir_f) != 1
		|| fwrite(htab, HTAB_SIZE * 4, 1, dir_f) != 1)
		return false;

	if (ftab) {
		if (fwrite(ftab, ftab_size, 1, dir_f) != 1)
			return false;
	}

	uint32_t header[2] = { ftell(dir_f), toc_offset };
	fseek(dir_f, 4, SEEK_SET);
	if (fwrite(header, 8, 1, dir_f) != 1)
		return false;

	fclose(dir_f);

	if (ftab) {
		free(ftab);
		ftab = NULL;
	}

	return true;
}
