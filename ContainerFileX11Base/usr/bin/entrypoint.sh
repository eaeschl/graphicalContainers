#!/bin/bash

function init_xdg()
{
        if test -z "${XDG_RUNTIME_DIR}"; then
                export XDG_RUNTIME_DIR=/tmp/${UID}-runtime-dir
                echo "XDG_RUNTIME_DIR=/tmp/${UID}-runtime-dir" >> /etc/environment
                if ! test -d "${XDG_RUNTIME_DIR}"; then
                        mkdir "${XDG_RUNTIME_DIR}"
                        chmod 0700 "${XDG_RUNTIME_DIR}"
                fi
        fi
}

function init()
{
        # echo error message, when executable file doesn't exist.
        if CMD=$(command -v "$1" 2>/dev/null); then
                shift
                export DISPLAY=:0

                exec "$CMD" "$@"
        else
                echo "Command not found: $1"

                # houston we have a problem
                exit 1
        fi
}

UDEV=$(echo "$UDEV" | awk '{print tolower($0)}')

case "$UDEV" in
        '1' | 'true')
                UDEV='on'
        ;;
esac

init_xdg
init "$@"
