# Nanover iMD

Interactive Molecular Dynamics (iMD) in VR, an application built with the NanoVer
framework.

This repository is maintained by the Intangible Realities Laboratory at the University of Santiago de Compostela,
and distributed under the [MIT](LICENSE) license.
See [the list of contributors](CONTRIBUTORS.md) for the individual authors of the project.

# Run the latest release build
* Download the [latest builds](https://github.com/IRL2/nanover-imd/releases).
* Extract the downloaded zip file.

* In the extracted directory,

## Android (Meta Quest etc)
* Sideload the `NanoVerIMD.apk` onto your device (you can use [SideQuest](https://sidequestvr.com/) for this).
* Look in the `Unknown Sources` section of your apps list and run NanoVer IMD.

## Windows (OpenXR / Meta Quest Link etc)
* In the extracted directory, launch `StandaloneWindows64.exe`. Windows will likely prompt you with a warning about the executable not being signed. If it happens, click on the "More info" button, then "Run anyway". You will also likely be prompted by the Windows firewall, allow NanoVer to access the network.

# Installation with conda

* Install Anaconda
* Open the "Anaconda Powershell Prompt" to enter the commands in the following instructions
* Create a conda environment (here we call the environment "nanover"): `conda create -n nanover "python>3.11"`
* Activate the conda environment: `conda activate nanover`
* Install the NanoVer IMD package: `conda install -c irl nanover-imd`
* Run the command `NanoverImd`

# Installation for Development

*  Clone this repository to a folder on your computer.
*  Update the submodules with `git submodule sync` and `git submodule update --init --recursive --remote`.
*  Download Unity Hub by visiting the [Unity Download Page](https://unity3d.com/get-unity/download) and clicking the green **Download Unity Hub** button.
*  Install Unity Hub onto your computer.
*  In Unity Hub, navigate to the **Projects** tab and click **Add** in the top right of Unity Hub.
*  Select the folder which you downloaded the repository to.
*  Install the version of Unity that Unity Hub tells you is required for this project.

Once open in Unity, the main Unity scene can be found in `NanoverIMD/Assets/NanoverXR Scene`.

## Citation, Credits and External Libraries

If you find this project useful, please cite the following paper: 

M. O’Connor, S.J. Bennie, H.M. Deeks, A. Jamieson-Binnie, A.J. Jones, R.J. Shannon, R. Walters, T. Mitchell, A.J. Mulholland, D.R. Glowacki, [“Interactive molecular dynamics from quantum chemistry to drug binding: an open-source multi-person virtual reality framework”](https://aip.scitation.org/doi/10.1063/1.5092590), J. Chem Phys 150, 224703 (2019)

This project has been made possible by the following projects. We gratefully thank them for their efforts, and suggest that you use and cite them:

* [unity](https://unity.com/) - Development platform.
* [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@2.1/manual/index.html) - Text rendering for Unity.
* [gRPC](https://grpc.io/) (Apache v2) - Communication protocol.
* [SteamVR SDK](https://github.com/ValveSoftware/steamvr_unity_plugin) (BSD) - SDK for developing VR applications in SteamVR.
* The CIF file importer uses the *Chemical Component Dictionary* provided at http://www.wwpdb.org/data/ccd.
* [icons8](https://icons8.com) - Provider of icons used in UI.
* [NSubstitute](https://nsubstitute.github.io/) (BSD) - Mock testing framework.
