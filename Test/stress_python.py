from locust import HttpUser, TaskSet, task, between

class UserBehavior(TaskSet):
    @task(1)
    def test_api(self):
        self.client.get("/comics")
        self.client.get("/comic/{key}")
        # self.client.get("/comic/{key}")
    @task(2)
    def test_api2(self):
        self.client.get("/comics")
        self.client.get("/comic/{key}")
        # self.client.get("/comic/{key}")

class WebsiteUser(HttpUser):
    tasks = [UserBehavior]
    wait_time = between(1, 2)  # wait time between tasks

if __name__ == "__main__":
    import os
    os.system("locust -f stress_python.py")
