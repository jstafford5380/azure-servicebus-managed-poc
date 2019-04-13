pipeline {
    agent any
    environment {
        HOME = '/tmp'
    }
    stages {
        stage('build') {
            steps {
                sh 'dotnet build src -c Release'
            }
        }
    }
}