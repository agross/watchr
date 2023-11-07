#!/bin/bash

# This program is free software. It comes without any warranty, to
# the extent permitted by applicable law. You can redistribute it
# and/or modify it under the terms of the Do What The Fuck You Want
# To Public License, Version 2, as published by Sam Hocevar. See
# http://sam.zoy.org/wtfpl/COPYING for more details.

for fgbg in 38 48; do
  for color in {0..255}; do
    printf '\e[%b;5;%bm  %3s  \e[0m' "$fgbg" "$color" "$color"

    # Display 6 colors per line.
    if (( (color + 1) % 6 == 4 )); then
      printf '\n'
    fi
  done
  printf '\n'
done
