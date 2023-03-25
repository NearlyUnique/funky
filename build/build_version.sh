BUILD_VERSION="$1"
PRERELEASE="$2"
DEBUG="$3"
SUFFIX=""

function log {
if [[ "$DEBUG" == "DEBUG" ]] ;
then
    echo $1 >&2
fi
}

log "INPUT BUILD_VERSION='$BUILD_VERSION' PRERELEASE='$PRERELEASE' SUFFIX='$SUFFIX'"

if [[ "$PRERELEASE" == "true" ]] ;
then
    SUFFIX="-preview"
fi


RX='v[0-9]\.[0-9]+\.[0-9]'
if  [[ $BUILD_VERSION =~ $RX ]] ;
then
    if [[ "${BUILD_VERSION:0:1}" == "v" ]] ;
    then
        BUILD_VERSION=${BUILD_VERSION:1}
    fi
    log "semantic version $BUILD_VERSION"
else
    BUILD_VERSION="1.0.0-build"
    log "no semantic version $BUILD_VERSION"
fi

echo "${BUILD_VERSION}${SUFFIX}"
