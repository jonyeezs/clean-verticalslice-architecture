resource "aws_ecs_cluster" "this" {
  name = "${local.service}-cluster"
}

resource "aws_ecs_task_definition" "initiator" {
  family             = "${local.service}-task"
  network_mode       = "awsvpc"
  task_role_arn      = aws_iam_role.task.arn
  execution_role_arn = aws_iam_role.task.arn

  container_definitions = jsonencode([
    {
      "name" : "clean-slice",
      "image" : "httpd:2.4",
      "portMappings" : [
        {
          "containerPort" : 80,
          "hostPort" : 80,
          "protocol" : "tcp"
        }
      ],
      "logConfiguration" : {
        "logDriver" : "awslogs",
        "options" : {
          "awslogs-group" : "clean-slice-task",
          "awslogs-region" : "ap-southeast-2",
          "awslogs-stream-prefix" : "clean-slice",
          "awslogs-create-group" : "true"
        }
      },
      "essential" : true,
      "entryPoint" : [
        "sh",
        "-c"
      ],
      "command" : [
        "/bin/sh -c \"echo '<html> <head> <title>Temporary page</title> <style>body {margin-top: 40px; background-color: #333;} </style> </head><body> <div style=color:white;text-align:center> <h2>Congratulations!</h2> <p>Now ready for CICD.</p> </div></body></html>' >  /usr/local/apache2/htdocs/index.html && httpd-foreground\""
      ]
    }
  ])

  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
}

resource "aws_cloudwatch_log_group" "task" {
  name = "${local.service}-production"
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
