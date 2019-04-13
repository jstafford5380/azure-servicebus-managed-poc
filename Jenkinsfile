pipeline {
    agent any
    environment {
        HOME = '/tmp'
    }
    stages {
        stage('build') {
            steps {
                powershell 'dotnet build src -c Release'
            }
        }
    }
}