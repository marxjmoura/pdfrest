#!/bin/bash

set -e

basedir="$(dirname $0)/.."
solution_dir="$basedir/src"
api_project_dir="$solution_dir/PDFRest.API"
test_project_dir="$solution_dir/PDFRest.Tests"

dotnet clean $api_project_dir
dotnet clean $test_project_dir

rm -rf $api_project_dir/bin
rm -rf $api_project_dir/obj
rm -rf $test_project_dir/bin
rm -rf $test_project_dir/obj
rm -rf $test_project_dir/coverage

dotnet restore $api_project_dir
dotnet restore $test_project_dir
