pipeline {
	agent any
	
	environment {
		gitlab_user_pwd=credentials('gitlab-user-pwd')
		BuildConfiguration="Debug"
		ImageName="registry.gitlab.com/rayprosoft/turnkey-gaming-system/umpay"
	}

	stages {
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
			steps {
				sh 'dotnet pack -c $BuildConfiguration "Tgs.Payment.UmPay.Client/Tgs.Payment.UmPay.Client.csproj" --no-build'
				sh 'dotnet nuget push "Tgs.Payment.UmPay.Client/bin/$BuildConfiguration/*.nupkg" --source "raypro" --skip-duplicate'
			}
		}

		stage('Build Docker Image') {
			steps {
				sh 'dotnet build "Tgs.Payment.UmPay/Tgs.Payment.UmPay.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:v${BUILD_NUMBER}" --no-restore '
			}
		}

		stage('Push Docker Image') {
			steps {
				sh 'sudo docker push $ImageName:v${BUILD_NUMBER}'
			}
		}

		stage('Deploy to dev') {
			input {
				message 'Ready to go?'
				ok 'yes'
			}

			steps {
				configFileProvider([
					configFile(
						fileId: 'umpay.dev.json', 
						variable: 'umpaydev')]) {
					script {
						def json = readJSON file: "$umpaydev";

						env.TGS = json.tgs;
						env.PaymentApiUrl = json.paymentApiUrl;
						env.NatsUri = json.nats;
						env.CallbackUrl = json.callbackBaseUrl;

						withKubeConfig([credentialsId: 'k8s-kube-config', serverUrl: 'https://192.168.50.105:6443']) {
							sh '''
								helm template k8s/umpay \
									--set image.tag=v${BUILD_NUMBER} \
									--set namespace=dev \
									--set "connectionStrings.tgs=${TGS}" \
									--set paymentApiUrl=${PaymentApiUrl} \
									--set nats=${NatsUri} \
									--set callbackBaseUrl=${CallbackUrl}
							'''
							sh '''
								helm template k8s/umpay \
									--set image.tag=v${BUILD_NUMBER} \
									--set namespace=dev \
									--set "connectionStrings.tgs=${TGS}" \
									--set paymentApiUrl=${PaymentApiUrl} \
									--set nats=${NatsUri} \
									--set callbackBaseUrl=${CallbackUrl} | kubectl -n dev apply -f -
							'''
						}
					}
				}
			}
		}
	}

	post {
		always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
	}
}