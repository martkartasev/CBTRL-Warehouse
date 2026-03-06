import os
from dataclasses import dataclass

import tyro

from cleanrl_ppo import Args as BaseArgs
from cleanrl_ppo import run_ppo
from unity_env_factory import make_unity_env_factory


@dataclass
class Args(BaseArgs):
    exp_name: str = os.path.basename(__file__)[: -len(".py")]
    env_id: str = "Move"
    total_timesteps: int = 100000000
    hidden_size: int = 256


make_env = make_unity_env_factory(
    base_port=50030,
    scene_load="LearnMove_Discrete",
    log_file_prefix="move_",  # Set to "" to disable logging files
)


if __name__ == "__main__":
    run_ppo(tyro.cli(Args), make_env)
