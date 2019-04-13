pipeline {
    agent any
    environment {
        HOME = '/tmp'
    }
    stages {
        stage('build') {
            steps {
                pwshFile('build.ps1')
            }
        }
    }
}

def pwshFile(script) {
    def command = "pwsh -File ${script}"
    sh command
}

def pwshScript(script) {
    def command = "pwsh -Command ${script}"
    sh command
}
