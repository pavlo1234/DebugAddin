#include "pch.h"

#include <Windows.h>
#include <cstdlib>
#include <cstdint>
#include <iostream>

int wmain(int argc, wchar_t ** argv)
  {
  if (argc != 3)
    {
    std::cerr << "Expected 2 arguments! library and proc\n";
    return 1;
    }    
  HMODULE library = LoadLibraryW(argv[1]);
  if (library == nullptr)
    {
    std::cerr << "LoadLibrary failure!\n";
    return 2;
    }    

  char ar[1024];
  size_t number_converted;
  wcstombs_s(&number_converted, ar, argv[2], 1024);
  void * proc_address = GetProcAddress(library, ar);

  if (proc_address == nullptr)
    {
    std::cerr << "Proc is not found!\n";
    return 3;
    }

  const std::uintptr_t offset = 
    reinterpret_cast<std::uintptr_t>(proc_address) -
    reinterpret_cast<std::uintptr_t>(library);

  std::cout << offset;

  FreeLibrary(library);

  return 0;
  }

