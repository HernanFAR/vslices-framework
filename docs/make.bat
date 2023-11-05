@ECHO OFF

pushd %~dp0

REM Command file for Sphinx documentation

set SOURCEDIR=source
set BUILDDIR=build

if errorlevel 9009 (
	echo.
	echo.The 'sphinx-build' command was not found. Make sure you have Sphinx
	echo.installed, then set the SPHINXBUILD environment variable to point
	echo.to the full path of the 'sphinx-build' executable. Alternatively you
	echo.may add the Sphinx directory to PATH.
	echo.
	echo.If you don't have Sphinx installed, grab it from
	echo.https://www.sphinx-doc.org/
	exit /b 1
)

if "%1" == "" goto help

if "%1" == "auto" (
	sphinx-build -M clean %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
	sphinx-autobuild %SOURCEDIR% %BUILDDIR%
	
	goto end
)

if "%1" == "auto_en" (
	sphinx-build -M clean %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
	sphinx-autobuild %SOURCEDIR% %BUILDDIR% -D language=en
	
	goto end
)

if "%1" == "update_locates" (
	sphinx-build -M clean %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
	sphinx-build -b gettext %SOURCEDIR% %BUILDDIR%/pot
	sphinx-intl update -p %BUILDDIR%/pot -l es -l en
	
	goto end
)

sphinx-build -M %1 %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
goto end

:help
sphinx-build -M help %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%

:end
popd
