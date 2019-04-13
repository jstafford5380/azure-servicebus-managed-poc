pipeline {
    agent any
    environment {
        HOME = '/tmp'
    }
    stages {
        stage('build') {
            steps {
                sh 'pwsh build.ps'
            }
        }
    }
}