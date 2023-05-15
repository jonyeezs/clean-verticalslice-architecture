resource "aws_ecs_cluster" "this" {
  name = "${local.service}-cluster"
}

resource "aws_ecs_task_definition" "initiator" {
  family             = "${local.service}-task"
  network_mode       = "awsvpc"
  execution_role_arn = aws_iam_role.task_execution.arn

  container_definitions = jsonencode([
    {
      "name" : "clean-slice",
      "image" : "nginxdemos/hello:0.3",
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
      "essential" : true
    }
  ])

  requires_compatibilities = ["FARGATE"]
  cpu                      = 1
  memory                   = 256
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
