pipeline {
    agent any
    stages {
        stage('build') {
            steps {
                sh 'dotnet build src -c Release'
            }
        }
    }
}