namespace MiniScaffold.Tests
open System.IO
open Expecto
open Infrastructure

module Assert =
    open System


    let private failIfNoneWithMsg msg opt =
        match opt with
        | Some _ -> ()
        | None -> failtest msg

    let private tryFindFile file (d : DirectoryInfo) =
        let filepath = Path.Combine(d.FullName, file)
        if  filepath |> File.Exists |> not then
            failtestf "Could not find %s" filepath

    let ``project can build target`` target (d : DirectoryInfo) =
        Builds.executeBuild d.FullName target

    let ``CHANGELOG exists`` =
        tryFindFile "CHANGELOG.md"

    let ``CHANGELOG contains Unreleased section`` (d : DirectoryInfo) =
        let changelogContents = Path.Combine(d.FullName, "CHANGELOG.md") |> File.ReadAllLines
        Expect.contains changelogContents "## [Unreleased]" "Changelog should contain Unreleased section"

    let ``CHANGELOG does not contain Unreleased section`` (d : DirectoryInfo) =
        let changelogContents = Path.Combine(d.FullName, "CHANGELOG.md") |> File.ReadAllLines
        Expect.isFalse (changelogContents |> Array.contains "## [Unreleased]") "Changelog should not contain Unreleased section"

    let ``.config/dotnet-tools.json exists`` =
        tryFindFile ".config/dotnet-tools.json"

    let ``.github ISSUE_TEMPLATE bug_report exists`` =
        tryFindFile ".github/ISSUE_TEMPLATE/bug_report.md"

    let ``.github ISSUE_TEMPLATE feature_request exists`` =
        tryFindFile ".github/ISSUE_TEMPLATE/feature_request.md"

    let ``.github workflows build exists`` =
        tryFindFile ".github/workflows/build.yml"

    let ``.github ISSUE_TEMPLATE exists`` =
        tryFindFile ".github/ISSUE_TEMPLATE.md"

    let ``.github PULL_REQUEST_TEMPLATE exists`` =
        tryFindFile ".github/PULL_REQUEST_TEMPLATE.md"

    let ``.editorconfig exists`` =
        tryFindFile ".editorconfig"

    let ``.gitattributes exists`` =
        tryFindFile ".gitattributes"

    let ``.gitignore exists`` =
        tryFindFile ".gitignore"

    let ``LICENSE exists`` =
        tryFindFile "LICENSE.md"

    let ``paket.lock exists`` =
        tryFindFile "paket.lock"

    let ``paket.dependencies exists`` =
        tryFindFile "paket.dependencies"

    let ``README exists`` =
        tryFindFile "README.md"


module Effect =
    open System
    open Fake.IO

    let ``build target with failure in`` target (failureFunction : string) (d : DirectoryInfo) =
        let buildScript = Path.combine d.FullName "build.fsx"
        buildScript |> File.applyReplace (fun text ->
            text.Replace(sprintf "let %s _ =\n" failureFunction,
                         sprintf "let %s _ =\n    failwith \"Deliberate failure in unit test\"\n" failureFunction)
        )
        try
            Builds.executeBuild d.FullName target
        with _ -> ()  // We expect failure here

    let ``set environment variable`` name value (d : DirectoryInfo) =
        Environment.SetEnvironmentVariable(name, value)
