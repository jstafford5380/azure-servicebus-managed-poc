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
    sh 'pwsh -File ${script}'
}

def pwshScript(script) {
    sh 'pwsh -Command ${script}'
}
