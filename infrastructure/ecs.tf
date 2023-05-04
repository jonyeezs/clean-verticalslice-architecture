resource "aws_ecs_cluster" "this" {
  name = "${local.service}-cluster"
}

resource "aws_ecs_task_definition" "initiator" {
  family             = "${local.service}-task"
  task_role_arn      = aws_iam_role.task.arn
  execution_role_arn = aws_iam_role.task.arn

  container_definitions = jsonencode(
    [{
      "name" : local.service
      "image" : "httpd:2.4",
      "portMappings" : [
        { containerPort = 80 }
      ],
      "entryPoint" : [
        "sh",
        "-c"
      ],
      "command" : [
        "/bin/sh -c \"echo '<html> <head> <title>Attention</title> <style>body {margin-top: 40px; background-color: #333;} </style> </head><body> <div style=color:white;text-align:center> <h1>${local.service}</h1>  <p>Your application is now running on a container in Amazon ECS. Allow pipeline to deploy application.</p> </div></body></html>' >  /usr/local/apache2/htdocs/index.html && httpd-foreground\""
      ]
    }]
  )
  cpu    = 256
  memory = 512

  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
}

resource "aws_ecs_service" "ecs_service" {
  name        = local.service
  cluster     = aws_ecs_cluster.this.id
  launch_type = "FARGATE"

  load_balancer {
    target_group_arn = aws_lb_target_group.this.arn
    container_port   = 80
    container_name   = local.service
  }

  task_definition = aws_ecs_task_definition.initiator.arn

  desired_count = 1

  network_configuration {
    subnets          = data.aws_subnets.this.ids
    security_groups  = [aws_security_group.this.id]
    assign_public_ip = true
  }

  depends_on = [aws_lb.this, aws_lb_listener_rule.this]

  lifecycle {
    ignore_changes = [
      task_definition,
      desired_count
    ]
  }
}
