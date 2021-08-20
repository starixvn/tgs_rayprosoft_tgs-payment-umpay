def message = "";
pipeline {
	agent any

	environment {
		BuildConfiguration = "Debug"
		ImageName = "registry.gitlab.com/rayprosoft/turnkey-gaming-system/umpay"
		GitUrl = "gitlab.com/rayprosoft/tgs-payment-umpay.git"
		ImageTagUrl = "https://gitlab.com/api/v4/projects/20478010/registry/repositories/2154485/tags"
	}
	stages {
		stage('Prepare'){
			steps {
				sh "echo 'Branch: $GIT_BRANCH'";
				script {
					switch(env.GIT_BRANCH)
                    {
                        // case ['main', 'origin/main']:
                        //     env.kube_ns = "prod";
                        //     env.k8s_config = 'stag-k8s-config';
                        //     break;
                        // case ['release', 'origin/release']:
                        //     env.kube_ns = "staging";
                        //     env.k8s_config = 'stag-k8s-config';
                        //     break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config = 'k8s-kube-config' 
                            break;
                    }
				}
				configFileProvider([
                    configFile(
                        fileId: 'tgs.' + env.kube_ns , 
                        variable: 'tgsconfig')]) {
                    script {
                        def envItems = readProperties file: "$tgsconfig";

                        envItems.each { key, value -> 
                            env."tgs_${key}" = value;
                        }

                        env.TAG = env.kube_ns + '-' + sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim()
                        message += "\nBranch: ${env.GIT_BRANCH}" 
                        message += "\nTag: ${env.TAG}"
                        message += "\nEnv: ${env.kube_ns}"
                    }
				}
                sh "echo tag: $TAG"
			}
		}

		stage('Build') {
			steps {
				sh 'dotnet build -c $BuildConfiguration'
			}
		}

		stage('Test') {
			steps {
				sh 'dotnet test -c $BuildConfiguration --logger trx --no-build'
			}
		}
		
		stage('Push packages') {
			when {
                expression {
                    return env.kube_ns == 'dev';
                }
            }

			steps {
				sh 'dotnet pack -c $BuildConfiguration "Tgs.Payment.UmPay.Client/Tgs.Payment.UmPay.Client.csproj" --no-build'
				sh 'dotnet nuget push "Tgs.Payment.UmPay.Client/bin/$BuildConfiguration/*.nupkg" --source "raypro" --skip-duplicate'
			}
		}

		stage('Build Docker Image') {
			steps {
				sh 'dotnet build "Tgs.Payment.UmPay/Tgs.Payment.UmPay.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:v${TAG}" --no-restore '
			}
		}

		stage('Push Docker Image') {
			steps {
				sh 'sudo docker push $ImageName:v${TAG}'
			}
		}

		stage('Deploy umpay') {			
			steps {
				withKubeConfig([credentialsId: env.k8s_config]) {
                    sh '''
						helm template k8s/umpay \
							--set image.tag=v${TAG} \
							--set namespace=dev \
							--set "connectionStrings.tgs=${TGS}" \
							--set paymentApiUrl=${PaymentApiUrl} \
							--set nats=${NatsUri} \
							--set callbackBaseUrl=${CallbackUrl}
					'''
					sh '''
						helm template k8s/umpay \
							--set image.tag=v${TAG} \
							--set namespace=dev \
							--set "connectionStrings.tgs=${TGS}" \
							--set paymentApiUrl=${PaymentApiUrl} \
							--set nats=${NatsUri} \
							--set callbackBaseUrl=${CallbackUrl} | kubectl -n dev apply -f -
					'''
                }
			}
		}

		stage('Add Tag') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }		
            steps {
                sshagent(['ssh-jenkins']) {
                    sh "git tag -a ${TAG} -m ${TAG}"
                    sh "git push ${GIT_URL} ${TAG} HEAD:${GIT_BRANCH}"
                }
            }
        }

		stage('Remove Image Tags') {					
            steps {	
			    withCredentials([string(credentialsId: "GitLabToken", variable: "GitLabToken")]) {					
                    sh "curl --request DELETE --data \'name_regex_delete=.*\' --data \'keep_n=10\' --header \'PRIVATE-TOKEN: ${GitLabToken}\' ${ImageTagUrl}"
                }
            }
        }	
	}

	post {
		always {
            step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
        }
        success {
             sendTelegram("Pipeline: ${currentBuild.fullDisplayName} OK! ${message} \n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        failure {
            sendTelegram("<b>Failed Pipeline:</b> ${currentBuild.fullDisplayName} Something is wrong with ${env.BUILD_URL}/console\n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }		
	}
}