pipeline {
    agent any
    environment {
        HOME = '/tmp'
    }
    stages {
        stage('Build') {
            parallel {
                stage('build and test') {
                    steps {
                        psFile('build.ps1')
                        echo 'MOCK: run tests'
                        echo 'MOCK: run analysis'
                    }
                }
                stage('publish artifacts') {
                    echo 'MOCK: publish binaries'                
                }
                stage('build docker container') {
                    echo 'MOCK: build docker container'
                }
            }
        },
        stage('Publish artifacts') {
            parallel {
                stage('Push binaries to artifactory') {
                    steps {
                        echo 'MOCK: upload artifacts'
                    }
                }
                stage('Push docker image') {
                    steps {
                        echo 'MOCK: push docker image'
                    }
                }
            }
        },
        stage('Deploy') {
            steps {
                echo 'MOCK: run helm'
            }
        }
    }
}

def psFile(script) {
    def command = "pwsh -File ${script}"
    sh command
}

def psCommand(script) {
    def command = "pwsh -Command ${script}"
    sh command
}
