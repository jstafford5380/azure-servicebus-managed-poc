pipeline {
    agent any
    environment {
        HOME = '/tmp'
    }
    stages {
        stage('build') {
            steps {
                pwsh 'dotnet build src -c Release'
            }
        }
    }
}