@echo off
setlocal enabledelayedexpansion

REM 시작 폴더 설정 (현재 폴더 기준)
set "folder=%cd%"

REM 총 줄 수를 저장할 변수 초기화
set total=0

echo 폴더: %folder%
echo ----------------------------

REM 모든 .cs 파일을 찾고 obj 폴더가 경로에 포함되지 않으면 줄 수를 셈
for /r "%folder%" %%f in (*.cs) do (
    echo %%f | findstr /i "\\obj\\" >nul
    if errorlevel 1 (
        for /f %%c in ('find /v /c "" ^< "%%f"') do (
            :: echo %%f : %%c 줄
            set /a total+=%%c
        )
    )
)

echo ----------------------------
echo 총 줄 수: !total! 줄

pause
