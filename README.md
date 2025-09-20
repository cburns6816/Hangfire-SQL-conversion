[[_TOC_]]


# 1. Setting up your local development environment
## 1.1. Using Visual Studio 2022
* Ensure Visaul Studio 2022 is running locally, and you have the [.NET 5.0 target SDK](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) installed
* Ensure that Powershell 7.4.1 is installed locally.
* Clone the repository using your preferred manner of interacting with Git (VS2022, git cli, GitKraken, etc)
* Create a local branch to work from. Suggested name: working/{username}. Example: working/zway
* Start making the changes you need. Recommendation is that changes are small, and committed often.
* For assistance, see [Microsoft Documentation](https://learn.microsoft.com/en-us/azure/devops/repos/git/pushing?view=azure-devops&tabs=visual-studio-2022)
* Go to [Creating a Pull Request](@creating-a-pull-request)

## 1.2. Using VS Code, and dotnet cli
* Ensure you have Visual Studio code installed, and the [.NET 5.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)
* Ensure you have [Git for Windows](https://git-scm.com/downloads) (or another git tool of your choice) setup and installed to your PATH
* Ensure you select the Git Credential Manager when you install Git for Windows. See further instructions [here](https://learn.microsoft.com/en-us/azure/devops/repos/git/share-your-code-in-git-cmdline?view=azure-devops)
* From a folder you are an owner of (recommend something like `D:\code` or `C:\code`), run `git clone https://lojic-gis-ags.visualstudio.com/Hangfire%20Orchestration/_git/Hangfire%20Orchestration` to pull the code
* Change to the newly created folder.
* Create your working branch `git checkout -b {branchname}`
* Launch vs code: `code .`
* Make your changes. To run the code from the cli, you can use `dotnet run`
* Commit your changes using `git commmit -a -m 'commit message'` to commit locally, then `git push` to push remotely
* For a more detailed walkthrough, and more turorials on how to use the Git CLI, see [Microsoft Documentation for Git CLI](https://learn.microsoft.com/en-us/azure/devops/repos/git/pushing?view=azure-devops&tabs=git-command-line)
* Go to [Creating a Pull Request](@creating-a-pull-request)

# 2. Getting changes pushed through the environments
## 2.1. Creating a Pull Request
* When you are at a point where your changes are ready to commit. Commit and push your changes and new branch to the remote
* Login to Azure DevOps
* Go to the Project Repository page
* You will see a banner at the top, stating you have changes in a branch, and asking if you'd like to create a "Pull Request", this is a shortcut you can use for the next step if you'd like.
* Go to the "[Pull Requests](https://lojic-gis-ags.visualstudio.com/_git/Hangfire%20Orchestration/pullrequests)" Page for the Hangfire.Orchestration repository
* Click the "New Pull Request" button in the upper right.
* The left drop down should have your branch selected. If not, change this branch to your personal branch. If it is missing, please ensure you pushed your changes from your local machine.
* The right drop down should have the master branch selected. If not, change this to the master branch
* Fill out the title for the Pull Request.
* I recommend importing your commit messages for the pull request message. Or you can override and fill out the message with whatever you wish.
* Click "Create Pull Request"
* For more details, and Documentation, see the [Microsoft Docs](https://learn.microsoft.com/en-us/azure/devops/repos/git/pull-requests?view=azure-devops&tabs=browser)

## 2.2. Pull Request Review
* Senior developers on the team must review the changes. 
* When at least 1 reviewer has approved the Pull Request, it can be completed.
* Once a pull request is completed, it will kick off a merge->build->release cycle
* When completing a pull request, the source branch should be deleted, and the merge type should be selected based on the desire to keep commit history or not.

## 2.3. Build and Release
* Once a pull request has been completed. The build will be kicked off automatically.
* The build process runs for Pull Requests, and Builds on the master branch
* The build is a multi-stage pipeline, controlled by the `azure-pipelines.yml` file in the repository.
* Stage 1:
    * Build the Project, and create an "Artifact". For more details, see [Azure DevOps Pipelines walkthrough](https://lojic-gis-ags.visualstudio.com/Hangfire%20Orchestration/_wiki/wikis/Hangfire-Orchestration.wiki/78/Azure-DevOps-Pipeline-Walkthrough)
* Stage 2 (Development):
    * In a `Rolling` deployment strategy, 2 servers are taken down, the code is updated, and they are then turned back on. Once they report healthy, we move to the next 2 servers.
* Future stages should be added for additional environments. Future stages should also usually require at least 1 Pre-Deployment approval.

# 3. Preparing to add a new server
## 3.1 Creating the confiugration file
* We are using the .NET Configuration extension that loads the config file based on the EnvironmentName, which is the Machine Name. So for each server in each enviornment, there is an `appsettings.{machineName}.json` file in the `LOJIC.Orchestration` folder.
* Create a new appsettings file, with the name of the machine you are going to configure.
* All non-sensitive configuration data should be loaded into this file. Use the base appsettings.json file as a guide for the options available.
* In the `Pipelines->Library` page on the left-nav of DevOps, you should find a variable group named `Hangfire_{Env}`, example: `Hangfire_Dev`
* This contains the secret values used by all machines in that environment, such as connection strings.
* This is a secure variable storage that is used during the deployment step to run variable replacement.
* Values in here are . reference replacements for the `appsettings.json` file. They run against all machines in an environment, so only override values that are valid for the entire enviornment here.
* For example, if you want to replace the connection string in a file that looks like this:
```json
{
    "ConnectionStrings": {
        "HangfireConnection": "Server=localhost\\SQLExpress;Database=Hangfire;User Id=hf_service;Password=>SuperSecure<"
    }
}
```
* Just set a variable for the environment in question named `ConnectionStrings.HangfireConnection` to the value you want. Then the appsettings.json file will have that value updated on deployment.

## 3.2 Creating a new Environment
* If you need a new environment, the process is a little more involved.
* First, go to the `Pipelines->Environments` page on the left nav in Azure DevOps
* Create your environment with it's new name. Then follow the steps on the [Wiki](https://lojic-gis-ags.visualstudio.com/Hangfire%20Orchestration/_wiki/wikis/Hangfire-Orchestration.wiki/76/Azure-DevOps-Agent-Setup-Guide) for registering all the target machines.
* Go to the `Pipelines->Library` page. Create a new Variable Group
* Copy the code of the last "Stage" in the azure-pipelines.yml file. This will give you a good starting point
* Edit the "environment:name", and "variables:group" values at the top of the stage. Change these to match your new Environment.
* Add any new steps, approvals, or post-approval steps that you wish
* Commit the changes to the `azure-pipelines.yml` file and create a Pull Request.
* PROFIT!